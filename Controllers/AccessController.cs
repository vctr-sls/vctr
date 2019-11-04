using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public AccessController(AppDbContext _db)
        {
            Db = _db;
        }

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

            return Redirect("/ui/manage");
        }

        [HttpGet("{shortIdent}")]
        public async Task<IActionResult> GetShortLink(string shortIdent, [FromQuery] bool disableTracking)
        {
            var shortLink = Db.ShortLinks.SingleOrDefault(sl => sl.ShortIdent == shortIdent);

            if (shortLink == null)
            {
                return Content(ErrorResponseContent.SHORT_LINK_NOT_FOUND, MediaTypeNames.Text.Plain);
            }

            if (!shortLink.IsActive || 
                shortLink.AccessCount >= shortLink.MaxUses ||
                shortLink.Activates.CompareTo(DateTime.Now) < 0 ||
                shortLink.Expires.CompareTo(DateTime.Now) >= 0)
            {
                return RedirectPreserveMethod("/ui/error/invalid");
            }

            if (shortLink.IsPasswordProtected)
            {
                return RedirectPreserveMethod("/ui/protected");
            }

            if (!disableTracking)
            {
                await CountRedirect(shortLink);
            }

            if (shortLink.IsPermanentRedirect)
            {
                return RedirectPermanent(shortLink.RootURL);
            }
            else
            {
                return RedirectPreserveMethod(shortLink.RootURL);
            }
        }

        public async Task CountRedirect(ShortLinkModel shortLink)
        {
            shortLink.Access();
            Db.ShortLinks.Update(shortLink);

            await Db.SaveChangesAsync();
        }
    }
}