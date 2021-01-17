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
    public class LinksController : AuthorizedControllerBase
    {
        private readonly IDatabaseAccess database;
        private readonly ICacheAccess cache;
        private readonly IPasswordHashingService hasher;

        public LinksController(
            IDatabaseAccess _database, 
            ICacheAccess _cache, 
            IPasswordHashingService _hasher)
        {
            database = _database;
            cache = _cache;
            hasher = _hasher;
        }

        // -------------------------------------------------------------------------
        // --- GET /api/links ---

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LinkViewModel>>> GetLinks(
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 100)
        {
            var canViewLinks = AuthorizedUser.HasPermissions(Permissions.VIEW_LINKS);

            var links = await database.GetAll<LinkModel>()
                .Where(l => canViewLinks || l.Creator.Guid == AuthorizedUser.Guid)
                .OrderByDescending(t => t.Created)
                .Skip(offset)
                .Take(limit)
                .Select(l => new LinkViewModel(l, AuthorizedUser))
                .ToListAsync();

            return Ok(links);
        }

        // -------------------------------------------------------------------------
        // --- GET /api/links/search ---

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<LinkViewModel>>> GetLinkSearch(
            [FromQuery] string query,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 100)
        {
            query = query.ToLower();
            var canViewLinks = AuthorizedUser.HasPermissions(Permissions.VIEW_LINKS);

            var links = await database.GetAll<LinkModel>()
                .Where(l => canViewLinks || l.Creator.Guid == AuthorizedUser.Guid)
                .Where(l => l.Ident.ToLower().Contains(query) || l.Destination.ToLower().Contains(query))
                .OrderByDescending(t => t.Created)
                .Skip(offset)
                .Take(limit)
                .Select(l => new LinkViewModel(l, AuthorizedUser))
                .ToListAsync();

            return Ok(links);
        }

        // -------------------------------------------------------------------------
        // --- POST /api/links ---

        [HttpPost]
        [RequiresPermission(Permissions.CREATE_LINKS)]
        public async Task<ActionResult<LinkViewModel>> Create([FromBody] LinkCreateModel link)
        {
            if (string.IsNullOrEmpty(link.Ident))
            {
                do link.Ident = RandomUtil.GetString(Constants.RandomIdentLength, Constants.RandomIdentChars); 
                while (!await ValidateIdent(link.Ident));
            } 
            else if (!await ValidateIdent(link.Ident))
                return BadRequest(new ResponseErrorModel("ident already exists"));

            if (!await LinksUtil.ValidateDestination(link.Destination))
                return BadRequest(new ResponseErrorModel("invalid destination url"));

            var newLink = new LinkModel
            {
                Creator = AuthorizedUser,
                Destination = link.Destination,
                Enabled = link.Enabled,
                Expires = link.Expires,
                Ident = link.Ident,
                PasswordHash = string.IsNullOrEmpty(link.Password) 
                    ? null 
                    : await hasher.GetEncodedHash(link.Password),
                PermanentRedirect = link.PermanentRedirect,
                TotalAccessLimit = link.TotalAccessLimit,
            };

            database.Create(newLink);
            await database.Commit();

            return Created(
                $"/api/links/{newLink.Guid}",
                new LinkViewModel(newLink, AuthorizedUser));
        }

        // -------------------------------------------------------------------------
        // --- GET /api/links/:id ---

        [HttpGet("{id}")]
        public async Task<ActionResult<LinkViewModel>> Get([FromRoute] Guid id)
        {
            var link = await database.GetById<LinkModel>(id);
            if (link == null)
                return NotFound();

            if (!Can(Permissions.VIEW_LINKS, link))
                return NotFound();

            return Ok(new LinkViewModel(link, AuthorizedUser));
        }

        // -------------------------------------------------------------------------
        // --- POST /api/links/:id ---

        [HttpPost("{id}")]
        public async Task<ActionResult<LinkViewModel>> Update(
            [FromRoute] Guid id,
            [FromBody] LinkUpdateModel newLink)
        {
            var link = await database.GetById<LinkModel>(id);
            if (link == null)
                return NotFound();

            if (!Can(Permissions.DELETE_LINKS, link))
                return NotFound();

            if (!string.IsNullOrEmpty(newLink.Destination) && newLink.Destination != link.Destination)
            {
                if (!(await LinksUtil.ValidateDestination(newLink.Destination)))
                    return BadRequest(new ResponseErrorModel("invalid destination url"));
                link.Destination = newLink.Destination;
            }

            if (!string.IsNullOrEmpty(newLink.Ident) && newLink.Ident != link.Ident)
            {
                if (!await ValidateIdent(newLink.Ident))
                    return BadRequest(new ResponseErrorModel("ident already exists"));
                link.Ident = newLink.Ident;
            }

            if (!string.IsNullOrEmpty(newLink.Password))
                link.PasswordHash = await hasher.GetEncodedHash(newLink.Password);

            link.Enabled = newLink.Enabled;
            link.PermanentRedirect = newLink.PermanentRedirect;
            link.TotalAccessLimit = newLink.TotalAccessLimit;
            link.Expires = newLink.Expires;

            database.Update(link);
            await database.Commit();
            await cache.Remove<LinkModel>(link.Ident);

            return Ok(new LinkViewModel(link, AuthorizedUser));
        }

        // -------------------------------------------------------------------------
        // --- DELETE /api/links/:id ---

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            var link = await database.GetById<LinkModel>(id);
            if (link == null)
                return NotFound();

            if (!Can(Permissions.DELETE_LINKS, link))
                return NotFound();

            var accesses = await database.GetWhere<AccessModel>(a => a.Link.Guid == id).ToArrayAsync();
            database.DeleteRange(accesses);
            database.Delete(link);

            await database.Commit();
            await cache.Remove<LinkModel>(link.Ident);

            return NoContent();
        }

        // -------------------------------------------------------------------------
        // --- HELPERS ---

        private async Task<bool> ValidateIdent(string ident) =>
            await database.GetWhere<LinkModel>(l => l.Ident.Equals(ident)).FirstOrDefaultAsync() == null;

        private bool Can(Permissions perm, LinkModel link) =>
            AuthorizedUser.HasPermissions(perm) || link.Creator.Guid == AuthorizedUser.Guid;
    }
}
