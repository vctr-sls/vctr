using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Net;

namespace RESTAPI.Filter
{
    /// <summary>
    /// Action filter whcih replaces the RemoteIPAddress of the
    /// current HTTP connection when the "X-Forwarded-For" header
    /// was detected and has a valid value.
    /// </summary>
    public sealed class ProxyAddress : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpCtx = context.HttpContext;

            httpCtx.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues headerValues);

            var val = headerValues.FirstOrDefault();
            if (val != null && val != "")
                context.HttpContext.Connection.RemoteIpAddress = IPAddress.Parse(val);
        }
    }
}
