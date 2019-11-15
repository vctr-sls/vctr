using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using slms2asp.Database;
using slms2asp.Extensions;
using slms2asp.Models;
using slms2asp.Shared;

namespace slms2asp.Controllers
{
    // ----------------------------------------------------
    // -- Access Controller
    // -- /{shortIdent}
    // --
    // -- The actual controller for accesses of short links
    // -- which controlls the redirection flow.

    [Route("")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly AppDbContext Db;
        private readonly IPCache IPCache;
        private readonly IConfiguration Configuration;
        private readonly ILogger<Startup> Logger;

        public AccessController(AppDbContext db, IPCache ipCache, IConfiguration configuration, ILogger<Startup> logger)
        {
            Db = db;
            IPCache = ipCache;
            Configuration = configuration;
            Logger = logger;
        }

        // ------------------------------------------------
        // GET /
        //
        // Redirects to the initialization page where you
        // enter your password on first startup. Defaultly,
        // this redirect either to the web interface
        // entrypoint or to a default page, if configured.

        [HttpGet]
        public IActionResult GetDefault()
        {
            var settings = Db.GeneralSettings.FirstOrDefault();

            if (settings == null || settings.PasswordHash.IsEmpty())
            {
                return Redirect("/ui/initialize");
            }

            if (!settings.DefaultRedirect.IsEmpty())
            {
                return Redirect(settings.DefaultRedirect);
            }

            return Redirect("/ui");
        }

        // ------------------------------------------------
        // GET /:shortIdent
        //                 ?disableTracking={bool}
        //
        // The basic entrypoint for each defined short
        // link. This methods tries to get the short link
        // object from the database by its identifier and
        // decides if the user is allowed to be redirected
        // to the root link set for this short link.
        // 
        // If the short link is set as 'inavtive', the
        // activation date is not reached yet, the maximum
        // access count is exceed or the expration date is
        // reached, this will redirect to the invalid short
        // link error page of the web UI.
        //
        // If the short link has a set password, this will
        // redirect to the password form of the web UI with
        // the short link GUID as URL query.
        //
        // If the URL query parameter 'disableTracking' is
        // passed as 'true', this access will not be logged
        // in the access log table. This does not mean, that
        // the access count will not be increased and this
        // wil not prevent vctr from saving the access remote
        // address in the in-memory address cache to prevent
        // bypassing the access count limit.

        [HttpGet("{shortIdent}")]
        public async Task<IActionResult> GetShortLink(string shortIdent, [FromQuery] bool disableTracking)
        {
            var shortLink = Db.ShortLinks
                .SingleOrDefault(sl => sl.ShortIdent == shortIdent);

            if (!IsValid(shortLink))
            {
                return RedirectPreserveMethod("/ui/error/invalid");
            }

            if (shortLink.IsPasswordProtected)
            {
                var url = $"/ui/protected?guid={shortLink.GUID}";
                if (disableTracking)
                {
                    url += "&disableTracking=true";
                }

                return RedirectPreserveMethod(url);
            }

            var addr = HttpContext.Connection.RemoteIpAddress;

            await CountRedirect(shortLink, disableTracking);

            if (shortLink.IsPermanentRedirect)
            {
                return RedirectPermanent(shortLink.RootURL);
            }
            else
            {
                return RedirectPreserveMethod(shortLink.RootURL);
            }
        }

        [HttpPost("protectedredirect/{guid}")]
        public async Task<IActionResult> ProtectedRedirect(Guid guid, [FromBody] ProtectedPostModel model)
        {
            if (model.Password.IsEmpty())
            {
                return Unauthorized();
            }

            var shortLink = Db.ShortLinks.Find(guid);

            if (!IsValid(shortLink))
            {
                return BadRequest(ErrorModel.BadRequest("invalid short link"));
            }

            if (!shortLink.IsPasswordProtected)
            {
                return BadRequest(ErrorModel.BadRequest("short link not password protected"));
            }

            try
            {
                if (!Hashing.CompareStringToHash(model.Password, shortLink.PasswordHash))
                {
                    return Unauthorized();
                }
            }
            catch (Exception)
            {
                return Unauthorized();
            }

            await CountRedirect(shortLink, model.DisableTracking);

            var res = new ProtectedResponseModel()
            {
                RootURL = shortLink.RootURL,
            };

            return Ok(res);
        }

        // ------------------------------------------------
        // -- HELPERS

        public bool IsValid(ShortLinkModel shortLink) =>
            shortLink != null &&
            shortLink.IsActive &&
            (shortLink.MaxUses <= 0 || shortLink.UniqueAccessCount < shortLink.MaxUses) &&
            shortLink.Activates.CompareTo(DateTime.Now) < 0 &&
            shortLink.Expires.CompareTo(DateTime.Now) >= 0;

        public async Task CountRedirect(ShortLinkModel shortLink, bool disableTracking)
        {
            var addr = HttpContext.Connection.RemoteIpAddress;

            bool isUnique = true;

            if (IPCache.Contains(addr))
            {
                var guid = IPCache.Get(addr);
                isUnique = !guid.HasValue || guid.Value != shortLink.GUID;
            }

            Db.ShortLinks.Update(shortLink);

            var ipInfoToken = Configuration.GetSection("secrets")?.GetValue<string>("ipinfotoken");
            var ipInfo = new IPInfoModel();

            if (!ipInfoToken.IsEmpty())
            {
                try
                {
                    ipInfo = await IPInfo.GetInfo(addr, ipInfoToken);
                }
                catch (Exception e)
                {
                    Logger.LogError("Failed getting IPInfo: ", e);
                }
            }
            else
            {
                Logger.LogWarning("IPInfo could not be collected because no IPInfo API token was provided");
            }

            var access = new AccessModel()
            {
                ShortLinkGUID = shortLink.GUID,
                IsUnique = isUnique,
                Timestamp = DateTime.Now,
            };


            if (!disableTracking)
            {
                access.ShortLinkGUID = shortLink.GUID;
                access.City = ipInfo.City;
                access.Country = ipInfo.Country;
                access.IsUnique = isUnique;
                access.Org = ipInfo.Org;
                access.Postal = ipInfo.Postal;
                access.Region = ipInfo.Region;
                access.Timestamp = DateTime.Now;
                access.Timezone = ipInfo.Timezone;
                access.UserAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            }

            Db.Accesses.Add(access);

            if (isUnique)
            {
                IPCache.Push(addr, shortLink.GUID);
            }

            await Db.SaveChangesAsync();
        }
    }
}