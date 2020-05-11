﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using slms2asp.Database;
using slms2asp.Extensions;
using slms2asp.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;

namespace slms2asp.Controllers
{
    // ----------------------------------------------------
    // -- Authorization Controller
    // -- /api/authorization

    [Route("api/[controller]/[action]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class AuthorizationController : ControllerBase
    {
        private readonly AppDbContext Db;

        public AuthorizationController(AppDbContext db)
        {
            Db = db;
        }

        // ------------------------------------------------
        // POST /api/authorization/login
        // 
        // Checks the passed 'Authorization' header value
        // for a valid password which is passed as basic
        // token:
        //   Authorization: Baisc passwordString
        // 
        // If the passed authorization token is valid, a
        // session token will be set which is used for
        // further authorization.

        [HttpPost]
        public async Task<IActionResult> Login()
        {
            if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeaderValue))
            {
                return Unauthorized();
            }

            if (authHeaderValue.Count <= 0 || !authHeaderValue[0].ToLower().StartsWith("basic "))
            {
                return Unauthorized();
            }

            var authValue = authHeaderValue[0].Substring(6);

            var settings = Db.GeneralSettings.FirstOrDefault();

            if (settings == null || settings.PasswordHash.IsEmpty())
            {
                return Unauthorized();
            }

            if (!Hashing.CompareStringToHash(authValue, settings.PasswordHash))
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
            {};

            var identity = new ClaimsIdentity(claims, "login");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);

            return Ok();
        }

        // ------------------------------------------------
        // POST /api/authorization/logout
        // 
        // Removes an authorization cookie, if existent.

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok();
        }
    }
}