using Microsoft.AspNetCore.Mvc.Filters;

namespace slms2asp.Filters
{
    /// <summary>
    /// 
    /// Action Filter set for every incomming request which
    /// sets specified response header values.
    /// 
    /// </summary>
    public class HeaderFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context) {}

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var responseHeaders = context.HttpContext.Response.Headers;

            responseHeaders.Add("Server", "vctr");
        }
    }
}
