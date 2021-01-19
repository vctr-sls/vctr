using DatabaseAccessLayer;
using DatabaseAccessLayer.Models;
using Gateway.Filter;
using Gateway.Models;
using Gateway.Services.Hashing;
using Gateway.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Gateway.Controllers.Endpoints
{
    [Route("api/[controller]")]
    [TypeFilter(typeof(RequiresAuth))]
    [ApiController]
    public class ApiKeyController : AuthorizedControllerBase
    {
        private readonly IDatabaseAccess database;
        private readonly IApiKeyHashingService hasher;

        public ApiKeyController(
            IDatabaseAccess _database,
            IApiKeyHashingService _hasher)
        {
            database = _database;
            hasher = _hasher;
        }

        // -------------------------------------------------------------------------
        // --- POST /api/apikey ---

        [HttpPost]
        [RequiresPermission(Permissions.CREATE_API_KEY)]
        public async Task<ActionResult<ApiKeyCreatedViewModel>> CreateApiKey()
        {
            var apiKey = await GetMyApiKey();
            if (apiKey != null)
                database.Delete(apiKey);

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

        // -------------------------------------------------------------------------
        // --- GET /api/apikey ---

        [HttpGet]
        public async Task<ActionResult<ApiKeyViewModel>> GetApiKey()
        {
            var apiKey = await GetMyApiKey();

            if (apiKey == null)
                return NotFound();

            return Ok(new ApiKeyViewModel(apiKey));
        }

        // -------------------------------------------------------------------------
        // --- DELETE /api/apikey ---

        [HttpDelete]
        public async Task<ActionResult<ApiKeyViewModel>> DeleteApiKey()
        {
            var apiKey = await GetMyApiKey();

            if (apiKey != null)
            {
                database.Delete(apiKey);
                await database.Commit();
            }

            return Ok();
        }

        // -------------------------------------------------------------------------
        // --- HELPERS ---

        private Task<ApiKeyModel> GetMyApiKey() =>
            database.GetWhere<ApiKeyModel>(k => k.User.Guid == AuthorizedUser.Guid).FirstOrDefaultAsync();
    }
}
