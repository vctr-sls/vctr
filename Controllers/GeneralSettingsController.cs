using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using slms2asp.Database;
using slms2asp.Extensions;
using slms2asp.Models;
using slms2asp.Shared;

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

        public GeneralSettingsController(AppDbContext _db)
        {
            Db = _db;
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
                settings.PasswordHash = Hashing.CreatePasswordHash(model.Password);
            }

            if (!model.DefaultRedirect.IsEmpty())
            {
                if (!await URIValidation.Validate(model.DefaultRedirect))
                {
                    return BadRequest(ErrorModel.BadRequest("invalid defautl redirect url"));
                }
                settings.DefaultRedirect = model.DefaultRedirect;
            }

            if (isNew)
            {
                Db.GeneralSettings.Add(settings);
            }
            else
            {
                Db.GeneralSettings.Update(settings);
            }

            return Ok(settings);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Initialize([FromBody] GeneralSettingsPostModel model)
        {
            var settings = Db.GeneralSettings.FirstOrDefault();

            if (settings != null && !settings.PasswordHash.IsEmpty())
            {
                return Forbid();
            }

            if (model.Password.IsEmpty())
            {
                return BadRequest(ErrorModel.BadRequest("invalid password"));
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