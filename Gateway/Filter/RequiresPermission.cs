using DatabaseAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Gateway.Controllers;

namespace Gateway.Filter
{
    public sealed class RequiresPermission : ActionFilterAttribute
    {
        private readonly Permissions minRequiredPermissions;

        public RequiresPermission(Permissions minRequiredPermissions)
        {
            this.minRequiredPermissions = minRequiredPermissions;
        }

        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            var controller = ctx.Controller as IAuthorizedController;

            if (controller == null || controller.AuthorizedUser == null)
            {
                SetBadRequest(ctx);
                return;
            }

            if (!controller.AuthorizedUser.HasPermissions(minRequiredPermissions))
            {
                SetBadRequest(ctx);
                return;
            }

            base.OnActionExecuting(ctx);
        }

        private static void SetBadRequest(ActionExecutingContext ctx)
        {
            var result = new ObjectResult(new { message = "missing permissions" })
            {
                StatusCode = 400
            };
            ctx.Result = result;
        }
    }
}
