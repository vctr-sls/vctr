using DatabaseAccessLayer;
using DatabaseAccessLayer.Models;
using Gateway.Models;
using Gateway.Services.Authorization;
using Gateway.Services.Hashing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Gateway.Controllers.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static readonly TimeSpan DEFAULT_SESSION_EXPIRATION = TimeSpan.FromDays(1);
        private static readonly TimeSpan EXTENDED_SESSION_EXPIRATION = TimeSpan.FromDays(30);

        private readonly bool bypassSecureCookies;
        private readonly IAuthorizationService authorization;
        private readonly IDatabaseAccess database;
        private readonly IPasswordHashingService hasher;

        public AuthController(
            IAuthorizationService _authorization,
            IDatabaseAccess _database,
            IPasswordHashingService _hasher,
            IConfiguration config)
        {
            authorization = _authorization;
            database = _database;
            hasher = _hasher;

            bypassSecureCookies = config.GetValue(Constants.ConfigKeySessionsBypassSecureCookies, false);
        }

        // -------------------------------------------------------------------------
        // --- POST /api/auth/login ---

        [HttpPost("[action]")]
        public async Task<ActionResult<UserLoginViewModel>> Login(
            [FromBody] LoginModel login,
            [FromQuery] bool getSessionKey)
        {
            var user = await database
                .GetWhere<UserModel>(u => u.UserName.Equals(login.Ident) || string.IsNullOrEmpty(u.MailAddress) && u.MailAddress.Equals(login.Ident))
                .FirstOrDefaultAsync();

            if (user == null || !(await hasher.CompareEncodedHash(login.Password, user.PasswordHash))) 
                return Unauthorized();

            user.LastLogin = DateTime.Now;
            database.Update(user);

            var expire = login.Remember ? EXTENDED_SESSION_EXPIRATION : DEFAULT_SESSION_EXPIRATION;
            var claims = new AuthClaims
            {
                Guid = user.Guid,
            };

            var jwt = authorization.GetSessionKey(claims, expire);

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.Add(expire),
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = !bypassSecureCookies,
            };

            Response.Cookies.Append(Constants.SessionCookieName, jwt, cookieOptions);

            await database.Commit();

            var res = new UserLoginViewModel(user);
            if (getSessionKey)
                res.SessionKey = jwt;

            return Ok(res);
        }

        // -------------------------------------------------------------------------
        // --- POST /api/auth/logout ---

        [HttpPost("[action]")]
        public IActionResult Logout()
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now,
                HttpOnly = true,
            };

            Response.Cookies.Append(Constants.SessionCookieName, "", cookieOptions);

            return NoContent();
        }
    }
}
