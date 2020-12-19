using DatabaseAccessLayer;
using DatabaseAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Gateway.Controllers;
using Gateway.Services.Authorization;
using System;
using System.Threading.Tasks;
using CacheAccessLayer;
using Gateway.Services.Hashing;
using Microsoft.Extensions.Configuration;

namespace Gateway.Filter
{
    public sealed class ProcessLink : ActionFilterAttribute
    {
        private readonly IDatabaseAccess database;
        private readonly ICacheAccess cache;
        private readonly IHashingService fastHasher;

        private readonly TimeSpan cacheDuration;

        public ProcessLink(
            IConfiguration config, 
            IDatabaseAccess _database, 
            ICacheAccess _cache, 
            IHashingService _fastHasher)
        {
            database = _database;
            cache = _cache;
            fastHasher = _fastHasher;

            var cacheDurationStr = config.GetValue(Constants.ConfigKeyCacheDurationsLinks, "30.00:00:00");
            cacheDuration = TimeSpan.Parse(cacheDurationStr);
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext ctx, ResultExecutionDelegate next)
        {
            var controller = ctx.Controller as ILinkController;
            if (controller == null || controller.ProcessedLink == null)
                return;

            var link = controller.ProcessedLink;

            link.LastAccess = DateTime.Now;
            link.AccessCount++;

            var addr = ctx.HttpContext.Connection.RemoteIpAddress.ToString();
            Console.WriteLine(addr);
            //var hashedIPAddr = fastHasher.GetEncodedHash(addr);
            //database.GetWhere<AccessModel>(a => a.SourceIPHash)

            database.Update(link);
            await database.Commit();
            await cache.Set(link, link.Ident, cacheDuration);

            await Task.Delay(10_000);

            //await base.OnResultExecutionAsync(ctx, next);
        }
    }
}
