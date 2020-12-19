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
        private readonly IDatabaseAccess database;
        private readonly ICacheAccess cache;

        private readonly TimeSpan cacheDuration;

        public RedirectionController(
            IConfiguration config, 
            IHashingService _fastHasher,
            IDatabaseAccess _database, 
            ICacheAccess _cache)
        {
            fastHasher = _fastHasher;
            database = _database;
            cache = _cache;

            var cacheDurationStr = config.GetValue(Constants.ConfigKeyCacheDurationsLinks, "30.00:00:00");
            cacheDuration = TimeSpan.Parse(cacheDurationStr);
        }

        [HttpGet("{ident}")]
        [ProxyAddress]
        public async Task<ActionResult> Get(string ident)
        {
            var (ok, link) = await cache.Get<LinkModel>(ident);
            if (!ok)
            {
                link = await database.GetWhere<LinkModel>(l => l.Ident == ident).FirstOrDefaultAsync();
                if (link == null)
                    return NotFound();
            }

            var now = DateTime.Now;

            if (!link.Enabled ||
                link.Expires != default && link.Expires <= now || 
                link.TotalAccessLimit > 0 && link.TotalAccessLimit <= link.UniqueAccessCount)
                return NotFound();

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
    }
}
