using System;
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
    // ----------------------------------------------------
    //   Short Links Controller
    //   /api/shortlinks

    [Authorize]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class ShortLinksController : ControllerBase
    {
        private AppDbContext Db;

        public ShortLinksController(AppDbContext _db)
        {
            Db = _db;
        }

        // ------------------------------------------------
        // GET /api/shortlinks
        //                    ?page={pageNumber}
        //                    ?size={pageSize}
        //
        // Get a specified range of short link objects.

        [HttpGet]
        public IActionResult Get([FromQuery] int page = 0, [FromQuery] int size = 100)
        {
            var outList = Db.ShortLinks
                .Skip(page * size)
                .Take(size);
            return Ok(outList);
        }

        // ------------------------------------------------
        // POST /api/shortlinks
        //
        // Create a short link object.

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShortLinkModel shortLink)
        {
            if (shortLink.ShortIdent.IsEmpty())
            {
                return BadRequest(ErrorModel.BadRequest("invalid short ident"));
            }

            if (!CheckShortIdent(shortLink))
            {
                return BadRequest(ErrorModel.BadRequest("short ident already in use"));
            }

            if (!await shortLink.ValidateURI())
            {
                return BadRequest(ErrorModel.BadRequest("invalid root link"));
            }

            shortLink.Sanitize();

            Db.ShortLinks.Add(shortLink);
            await Db.SaveChangesAsync();

            return Created(shortLink.GUID.ToString(), shortLink);
        }

        // ------------------------------------------------
        // PUT /api/shortlinks/{guid}
        //
        // Replaces editable properties of a short link
        // object passed in the requests body.

        [HttpPut("{guid}")]
        public async Task<IActionResult> Edit(Guid guid, [FromBody] ShortLinkModel newShortLink)
        {
            var shortLink = Db.ShortLinks.Find(guid);

            if (shortLink == null)
            {
                return NotFound(ErrorModel.NotFound());
            }

            if (!CheckShortIdent(newShortLink))
            {
                return BadRequest(ErrorModel.BadRequest("short ident already in use"));
            }
            
            if (!await newShortLink.ValidateURI())
            {
                return BadRequest(ErrorModel.BadRequest("invalid root link"));
            }

            shortLink.Update(newShortLink);

            Db.ShortLinks.Update(shortLink);
            await Db.SaveChangesAsync();

            return Ok(shortLink);
        }

        // ------------------------------------------------
        // POST /api/shortlinks/{guid}/password
        //
        // Sets or resets the password state of a short
        // link object. If a password is set, password
        // check on access is automatically activated as
        // same as it is going to be disabled on reset.

        [HttpPost("{guid}/password")]
        public async Task<IActionResult> SetPassword(Guid guid, [FromBody] SetPasswordPostModel pwModel)
        {
            var shortLink = Db.ShortLinks.Find(guid);

            if (shortLink == null)
            {
                return NotFound(ErrorModel.NotFound());
            }

            if (pwModel.Reset)
            {
                shortLink.PasswordHash = default;
                return Ok();
            }

            if (pwModel.Password.IsEmpty())
            {
                return BadRequest(ErrorModel.BadRequest("empty password"));
            }

            shortLink.PasswordHash = Hashing.CreatePasswordHash(pwModel.Password);
            Db.ShortLinks.Update(shortLink);
            await Db.SaveChangesAsync();

            return Ok();
        }

        // -- HELPER ------------------------------------

        private bool CheckShortIdent(ShortLinkModel shortLink) =>
            Db.ShortLinks
                .Where(sl => sl.ShortIdent.Equals(shortLink.ShortIdent))
                .Count() == 0;
    }
}
