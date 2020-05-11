using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using slms2asp.Database;
using slms2asp.Extensions;
using slms2asp.Models;
using slms2asp.Shared;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace slms2asp.Controllers
{
    // TODO: Docs

    [Route("api/settings")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class GeneralSettingsController : ControllerBase
    {
        private readonly AppDbContext Db;

        public GeneralSettingsController(AppDbContext db)
        {
            Db = db;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            var settings = Db.GeneralSettings.FirstOrDefault();

            if (settings == null)
            {
                return NotFound(ErrorModel.NotFound());
            }

            return Ok(settings);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Set([FromBody] GeneralSettingsPostModel model)
        {
            var isNew = false;
            var settings = Db.GeneralSettings.FirstOrDefault();

            if (settings == null)
            {
                settings = new GeneralSettingsModel();
                isNew = true;
            }
            
            if (!model.Password.IsEmpty())
            {
                if (model.CurrentPassword.IsEmpty() || !Hashing.CompareStringToHash(model.CurrentPassword, settings.PasswordHash))
                {
                    return BadRequest(ErrorModel.BadRequest("invalid current password"));
                }

                settings.PasswordHash = Hashing.CreatePasswordHash(model.Password);
            }

            if (!model.DefaultRedirect.IsEmpty())
            {
                if (model.DefaultRedirect.Equals("__RESET__"))
                {
                    settings.DefaultRedirect = null;
                }
                else
                {
                    if (!await URIValidation.Validate(model.DefaultRedirect))
                    {
                        return BadRequest(ErrorModel.BadRequest("invalid defautl redirect url"));
                    }
                    settings.DefaultRedirect = model.DefaultRedirect;
                }

            }

            if (isNew)
            {
                Db.GeneralSettings.Add(settings);
            }
            else
            {
                Db.GeneralSettings.Update(settings);
            }

            await Db.SaveChangesAsync();

            return Ok(settings);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Initialize([FromBody] GeneralSettingsPostModel model)
        {
            var settings = Db.GeneralSettings.FirstOrDefault();

            if (settings != null && !settings.PasswordHash.IsEmpty())
            {
                return new ForbidResult();
            }

            if (model.Password.IsEmpty())
            {
                return BadRequest(ErrorModel.BadRequest("invalid new password"));
            }

            var hash = Hashing.CreatePasswordHash(model.Password);

            Db.GeneralSettings.Add(new GeneralSettingsModel()
            {
                PasswordHash = hash,
            });

            await Db.SaveChangesAsync();

            return Ok();
        }
    }
}