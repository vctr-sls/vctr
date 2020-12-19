using DatabaseAccessLayer;
using DatabaseAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RESTAPI.Controllers;
using RESTAPI.Services.Authorization;
using System;
using System.Threading.Tasks;

namespace RESTAPI.Filter
{
    public sealed class RequiresAuth : ActionFilterAttribute
    {
        private readonly IAuthorizationService authorization;
        private readonly IDatabaseAccess database;

        public RequiresAuth(IAuthorizationService _authorization, IDatabaseAccess _database)
        {
            authorization = _authorization;
            database = _database;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
        {
            var controller = ctx.Controller as IAuthorizedController;

            var ok = ctx.HttpContext.Request.Cookies.TryGetValue(Constants.SessionCookieName, out var token);
            if (controller == null || !ok || string.IsNullOrEmpty(token))
            {
                SetUnauthorized(ctx);
                return;
            }

            try
            {
                controller.AuthClaims = authorization.ValidateSessionKey(token);
                controller.AuthorizedUser = await database.GetById<UserModel>(controller.AuthClaims.Guid);
                if (controller.AuthorizedUser == null) throw new Exception();
            }
            catch
            {
                SetUnauthorized(ctx);
                return;
            }

            await base.OnActionExecutionAsync(ctx, next);
        }

        private static void SetUnauthorized(ActionExecutingContext ctx)
        {
            var result = new ObjectResult(null);
            result.StatusCode = 401;
            ctx.Result = result;
        }
    }
}
