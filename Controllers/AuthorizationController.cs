using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using slms2asp.Shared;
using System.Collections.Generic;
using System.Security.Claims;

namespace slms2asp.Controllers
{

    // TODO: Docs

    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly string PasswordHash;

        public AuthorizationController(IConfiguration configuration)
        {
            Configuration = configuration;
            PasswordHash = Configuration.GetValue<string>("Authorization:PasswordHash");
        }

        [HttpPost("[action]")]
        public IActionResult Login()
        {
            StringValues authHeaderValue;
            if (!HttpContext.Request.Headers.TryGetValue("Authorization", out authHeaderValue))
            {
                return Unauthorized();
            }

            if (authHeaderValue.Count <= 0 || !authHeaderValue[0].ToLower().StartsWith("basic "))
            {
                return Unauthorized();
            }

            var authValue = authHeaderValue[0].Substring(6);

            if (!Hashing.CompareStringToHash(authValue, PasswordHash))
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
            {};

            var identity = new ClaimsIdentity(claims, "login");
            var principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync(principal);

            return Ok();
        }
    }
}