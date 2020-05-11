using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using slms2asp.Extensions;
using System.Linq;
using System.Net;

namespace slms2asp.Filters
{
    public class ProxyAddressFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context) {}

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var httpCtx = context.HttpContext;

            httpCtx.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues headerValues);

            var val = headerValues.FirstOrDefault();
            if (!val.IsEmpty())
            {
                context.HttpContext.Connection.RemoteIpAddress = IPAddress.Parse(val);
            }
        }
    }
}
