using DatabaseAccessLayer;
using DatabaseAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gateway.Filter;
using Gateway.Models;
using Gateway.Services.Hashing;
using Gateway.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CacheAccessLayer;

namespace Gateway.Controllers.Endpoints
{
    [Route("api/[controller]")]
    [TypeFilter(typeof(RequiresAuth))]
    [ApiController]
    public class ApiKeyController : AuthorizedControllerBase
    {
        private readonly IDatabaseAccess database;
        private readonly IPasswordHashingService hasher;

        public ApiKeyController(
            IDatabaseAccess _database,
            IPasswordHashingService _hasher)
        {
            database = _database;
            hasher = _hasher;
        }

        // -------------------------------------------------------------------------
        // --- GET /api/apikey ---

        [HttpPost]
        [RequiresPermission(Permissions.CREATE_API_KEY)]
        public async Task<ActionResult<ApiKeyCreatedViewModel>> CreateApiKey()
        {
            var key = RandomUtil.GetString(Constants.RandomApiKeyLength, Constants.RandomApiKeyChars);

            var keyModel = new ApiKeyModel()
            {
                User = AuthorizedUser,
                KeyHash = await hasher.GetEncodedHash(key),
            };

            database.Create(keyModel);
            await database.Commit();

            return Created(
                $"/api/apikey/{keyModel.Guid}", 
                new ApiKeyCreatedViewModel(keyModel, key));
        }
    }
}
