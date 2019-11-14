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
    // -- Short Links Controller
    // -- /api/shortlinks

    [Authorize]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class ShortLinksController : ControllerBase
    {
        private readonly AppDbContext Db;

        public ShortLinksController(AppDbContext db)
        {
            Db = db;
        }

        // ------------------------------------------------
        // GET /api/shortlinks
        //                    ?page={int}
        //                    ?size={int}
        //                    ?sortBy={string}
        //
        // Get a specified range of short link objects.

        [HttpGet]
        public IActionResult Get([FromQuery] int page = 0, 
                                 [FromQuery] int size = 100, 
                                 [FromQuery] string sortBy = "CreationDate")
        {
            var shortLinkModel = typeof(ShortLinkModel);
            var property = shortLinkModel.GetProperties()
                .FirstOrDefault(p => p.Name.ToLower() == sortBy.ToLower());

            if (property == null)
            {
                return BadRequest(ErrorModel.BadRequest("invalid property for 'sortBy'"));
            }

            var outList = Db.ShortLinks
                .OrderByDescending(sl => property.GetValue(sl))
                .Skip(page * size)
                .Take(size);

            return Ok(outList);
        }

        // ------------------------------------------------
        // GET /api/shortlinks/:guid
        //
        // Get a specified short link object by GUID.

        [HttpGet("{guid}")]
        public IActionResult GetSingle(Guid guid)
        {
            var shortLink = Db.ShortLinks.Find(guid);

            if (shortLink == null)
            {
                return NotFound(ErrorModel.NotFound());
            }

            return Ok(shortLink);
        }

        // ------------------------------------------------
        // GET /api/shortlinks/search
        //                           ?query={string}
        //                           ?page={int}
        //                           ?size={int}
        //                           ?sortBy={string}
        //
        // Get a specified range of short link objects
        // filtered by search query.

        [HttpGet("search")]
        public IActionResult Search([FromQuery] string query, 
                                    [FromQuery] int page = 0, 
                                    [FromQuery] int size = 100, 
                                    [FromQuery] string sortBy = "CreationDate")
        {
            if (query.IsEmpty())
            {
                return BadRequest(ErrorModel.BadRequest("invalid search query"));
            }

            query = query.ToLower();

            var shortLinkModel = typeof(ShortLinkModel);
            var property = shortLinkModel.GetProperties()
                .FirstOrDefault(p => p.Name.ToLower() == sortBy.ToLower());

            if (property == null)
            {
                return BadRequest(ErrorModel.BadRequest("invalid property for 'sortBy'"));
            }

            var outList = Db.ShortLinks
                .Where((sl) => sl.ShortIdent.Contains(query) ||
                           sl.RootURL.Contains(query) ||
                           sl.GUID.ToString().Contains(query))
                .OrderByDescending(sl => property.GetValue(sl))
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
            if (!shortLink.ValidateIdent())
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

            shortLink.Sanitize(asNew: true);

            Db.ShortLinks.Add(shortLink);
            await Db.SaveChangesAsync();

            return Created(shortLink.GUID.ToString(), shortLink);
        }

        // ------------------------------------------------
        // PUT /api/shortlinks/:guid
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

            if (!shortLink.ValidateIdent())
            {
                return BadRequest(ErrorModel.BadRequest("invalid short ident"));
            }

            if (!CheckShortIdent(newShortLink, shortLink.GUID))
            {
                return BadRequest(ErrorModel.BadRequest("short ident already in use"));
            }
            
            if (!await newShortLink.ValidateURI())
            {
                return BadRequest(ErrorModel.BadRequest("invalid root link"));
            }

            shortLink.Update(newShortLink);
            shortLink.Sanitize();

            Db.ShortLinks.Update(shortLink);
            await Db.SaveChangesAsync();

            return Ok(shortLink);
        }

        // ------------------------------------------------
        // DELETE /api/shortlinks/:guid
        //
        // Deletes the short link object by the specified
        // GUID.

        [HttpDelete("{guid}")]
        public async Task<IActionResult> Delete(Guid guid)
        {
            var shortLink = Db.ShortLinks.Find(guid);

            if (shortLink == null)
            {
                return NotFound(ErrorModel.NotFound());
            }

            Db.ShortLinks.Remove(shortLink);
            await Db.SaveChangesAsync();

            return Ok();
        }

        // ------------------------------------------------
        // POST /api/shortlinks/:guid/password
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
                shortLink.IsPasswordProtected = false;
                return Ok();
            }

            if (pwModel.Password.IsEmpty())
            {
                return BadRequest(ErrorModel.BadRequest("empty password"));
            }

            shortLink.PasswordHash = Hashing.CreatePasswordHash(pwModel.Password);
            shortLink.IsPasswordProtected = true;

            Db.ShortLinks.Update(shortLink);
            await Db.SaveChangesAsync();

            return Ok();
        }

        // -- HELPER ------------------------------------

        private bool CheckShortIdent(ShortLinkModel shortLink) =>
            Db.ShortLinks
                .Where(sl => sl.ShortIdent.Equals(shortLink.ShortIdent) && !sl.GUID.Equals(shortLink.GUID))
                .Count() == 0;

        private bool CheckShortIdent(ShortLinkModel shortLink, Guid guid) =>
            Db.ShortLinks
                .Where(sl => sl.ShortIdent.Equals(shortLink.ShortIdent) && !sl.GUID.Equals(guid))
                .Count() == 0;
    }
}
