using DatabaseAccessLayer;
using DatabaseAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Gateway.Controllers;
using Gateway.Services.Authorization;
using System;
using System.Threading.Tasks;
using Gateway.Services.Hashing;
using Microsoft.EntityFrameworkCore;
using Gateway.Util;

namespace Gateway.Filter
{
    public sealed class RequiresAuth : ActionFilterAttribute
    {
        private readonly IAuthorizationService authorization;
        private readonly IDatabaseAccess database;
        private readonly IApiKeyHashingService hasher;

        public RequiresAuth(
            IAuthorizationService _authorization, 
            IDatabaseAccess _database,
            IApiKeyHashingService _hasher)
        {
            authorization = _authorization;
            database = _database;
            hasher = _hasher;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
        {
            var controller = ctx.Controller as IAuthorizedController;
            if (controller == null)
            {
                SetUnauthorized(ctx);
                return;
            }

            controller.AuthClaims = await TryGetAuthClaims(ctx);
            if (controller.AuthClaims == null)
            {
                SetUnauthorized(ctx);
                return;
            }

            controller.AuthorizedUser = await database.GetById<UserModel>(controller.AuthClaims.Guid);
            if (controller.AuthorizedUser == null)
            {
                SetUnauthorized(ctx);
                return;
            }

            await base.OnActionExecutionAsync(ctx, next);
        }

        private async Task<AuthClaims> TryGetAuthClaims(ActionExecutingContext ctx)
        {
            var ok = ctx.HttpContext.Request.Cookies.TryGetValue(Constants.SessionCookieName, out var token);
            if (ok && !string.IsNullOrEmpty(token))
                return FlowUtil.TryCatch(
                    () => authorization.ValidateSessionKey(token),
                    (_) => null);

            ok = ctx.HttpContext.Request.Headers.TryGetValue("authorization", out var apiKey);
            if (ok && !string.IsNullOrEmpty(apiKey))
            {
                var keyHash = await hasher.GetEncodedHash(apiKey);
                var apiKeyModel = await database.GetWhere<ApiKeyModel>(k => k.KeyHash == keyHash)
                    .Include(k => k.User).FirstOrDefaultAsync();

                if (apiKeyModel != null)
                {
                    apiKeyModel.AccessCount++;
                    apiKeyModel.LastAccess = DateTime.Now;
                    database.Update(apiKeyModel);
                    await database.Commit();

                    return new AuthClaims()
                    {
                        Guid = apiKeyModel.User.Guid,
                    };
                }
            }

            return null;
        }

        private static void SetUnauthorized(ActionExecutingContext ctx)
        {
            var result = new ObjectResult(null);
            result.StatusCode = 401;
            ctx.Result = result;
        }
    }
}
