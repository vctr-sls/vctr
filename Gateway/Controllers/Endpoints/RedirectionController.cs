using CacheAccessLayer;
using DatabaseAccessLayer;
using DatabaseAccessLayer.Models;
using Gateway.Filter;
using Gateway.Services.Hashing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Gateway.Controllers.Endpoints
{
    [Route("/")]
    [ApiController]
    public class RedirectionController : ControllerBase
    {
        private readonly IHashingService fastHasher;
        private readonly IPasswordHashingService passwordHasher;
        private readonly IDatabaseAccess database;
        private readonly ICacheAccess cache;

        private readonly TimeSpan cacheDuration;
        private readonly string routeRoot;
        private readonly string routeNotFound;
        private readonly string routePassword;

        public RedirectionController(
            IConfiguration config, 
            IHashingService _fastHasher,
            IPasswordHashingService _passwordHasher,
            IDatabaseAccess _database, 
            ICacheAccess _cache)
        {
            fastHasher = _fastHasher;
            passwordHasher = _passwordHasher;
            database = _database;
            cache = _cache;

            var cacheDurationStr = config.GetValue(Constants.ConfigKeyCacheDurationsLinks, "30.00:00:00");
            cacheDuration = TimeSpan.Parse(cacheDurationStr);

            routeRoot = config.GetValue<string>(Constants.ConfigKeyRoutingRoot);
            routeNotFound = config.GetValue<string>(Constants.ConfigKeyRoutingNotFound);
            routePassword = config.GetValue<string>(Constants.ConfigKeyRoutingPassword);
        }

        // -------------------------------------------------------------------------
        // --- GET / ---

        [HttpGet]
        public ActionResult Get() =>
            Route(NotFound, routeRoot);

        // -------------------------------------------------------------------------
        // --- GET /:ident ---

        [HttpGet("{ident}")]
        [ProxyAddress]
        public async Task<ActionResult> GetIdent(
            [FromRoute] string ident,
            [FromQuery] string password)
        {
            var (ok, link) = await cache.Get<LinkModel>(ident);
            if (!ok)
            {
                link = await database.GetWhere<LinkModel>(l => l.Ident == ident).FirstOrDefaultAsync();
                if (link == null)
                    return Route(NotFound, routeNotFound);
            }

            var now = DateTime.Now;

            if (!link.Enabled ||
                link.Expires != default && link.Expires <= now || 
                link.TotalAccessLimit > 0 && link.TotalAccessLimit <= link.UniqueAccessCount)
                return Route(NotFound, routeNotFound);

            if (!string.IsNullOrEmpty(link.PasswordHash))
            {
                if (string.IsNullOrEmpty(password))
                    return Route(BadRequest, "password required", routePassword);
                if (!await passwordHasher.CompareEncodedHash(password, link.PasswordHash))
                    return BadRequest("invalid password");
            }

            link.LastAccess = now;
            link.AccessCount++;

            var hashedIPAddr = await fastHasher.GetEncodedHash(HttpContext.Connection.RemoteIpAddress.ToString());
            if (await database.GetWhere<AccessModel>(a => a.SourceIPHash.Equals(hashedIPAddr)).FirstOrDefaultAsync() == null)
                link.UniqueAccessCount++;

            database.Create(new AccessModel
            {
                Link = link,
                SourceIPHash = hashedIPAddr,
            });
            database.Update(link);

            await database.Commit();
            await cache.Set(link, ident, cacheDuration);

            if (link.PermanentRedirect)
                return RedirectPermanent(link.Destination);

            return Redirect(link.Destination);
        }

        // -------------------------------------------------------------------------
        // --- HELPERS ---

        private ActionResult Route<T>(Func<T> result, string route) where T : ActionResult =>
            string.IsNullOrEmpty(route)
                ? result.Invoke()
                : Redirect(route);

        private ActionResult Route<TArg, TRes>(Func<TArg, TRes> result, TArg arg, string route) where TRes : ActionResult =>
            string.IsNullOrEmpty(route)
                ? result.Invoke(arg)
                : Redirect(route);
    }
}
