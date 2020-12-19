using DatabaseAccessLayer;
using DatabaseAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gateway.Filter;
using Gateway.Models;
using Gateway.Services.Hashing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway.Controllers.Endpoints
{
    [Route("api/[controller]")]
    [TypeFilter(typeof(RequiresAuth))]
    [ApiController]
    public class UsersController : AuthorizedControllerBase
    {
        private readonly IDatabaseAccess database;
        private readonly IPasswordHashingService hasher;

        public UsersController(IDatabaseAccess _database, IPasswordHashingService _hasher)
        {
            database = _database;
            hasher = _hasher;
        }

        // -------------------------------------------------------------------------
        // --- GET /api/users ---

        [HttpGet]
        [RequiresPermission(Permissions.VIEW_USERS)]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> GetUsers(
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 100)
        {
            var users = await database.GetAll<UserModel>()
                .OrderBy(t => t.Created)
                .Skip(offset)
                .Take(limit)
                .Select(u => new UserViewModel(u))
                .ToListAsync();

            return Ok(users);
        }

        // -------------------------------------------------------------------------
        // --- POST /api/users ---

        [HttpPost]
        [RequiresPermission(Permissions.CREATE_USERS)]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> Create([FromBody] UserCreateModel user)
        {
            var existingUser = await database.GetWhere<UserModel>(u => u.UserName.Equals(user.UserName)).FirstOrDefaultAsync();
            if (existingUser != null)
                return BadRequest("username already exists");

            var newUser = new UserModel
            {
                UserName = user.UserName,
                PasswordHash = await hasher.GetEncodedHash(user.Password),
                Permissions = user.Permissions,
            };

            database.Create(newUser);
            await database.Commit();

            return Ok(new UserViewModel(newUser));
        }

        // -------------------------------------------------------------------------
        // --- GET /api/users/me ---

        [HttpGet("me")]
        public ActionResult<UserViewModel> Me() =>
            Ok(new UserViewModel(AuthorizedUser));

        // -------------------------------------------------------------------------
        // --- POST /api/users/me ---

        [HttpPost("me")]
        public Task<ActionResult<UserViewModel>> PostMe([FromBody] UserUpdateModel newUser) =>
            Update(AuthorizedUser.Guid, newUser);

        // -------------------------------------------------------------------------
        // --- GET /api/users/:id ---

        [HttpGet("{id}")]
        [RequiresPermission(Permissions.VIEW_USERS)]
        public async Task<ActionResult<UserViewModel>> Get([FromRoute] Guid id)
        {
            var user = await database.GetById<UserModel>(id);
            if (user == null)
                return NotFound();

            return Ok(new UserViewModel(user));
        }

        // -------------------------------------------------------------------------
        // --- POST /api/users/:id ---

        [HttpPost("{id}")]
        [RequiresPermission(Permissions.UPDATE_USERS)]
        public async Task<ActionResult<UserViewModel>> Update(
            [FromRoute] Guid id,
            [FromBody] UserUpdateModel newUser)
        {
            var user = await database.GetById<UserModel>(id);
            if (user == null)
                return NotFound();

            if (!string.IsNullOrEmpty(newUser.UserName))
                user.UserName = newUser.UserName;

            if (!string.IsNullOrEmpty(newUser.Password))
                user.PasswordHash = await hasher.GetEncodedHash(newUser.Password);

            if (newUser.Permissions != Permissions.UNSET)
                user.Permissions = newUser.Permissions;

            database.Update(user);
            await database.Commit();

            return Ok(new UserViewModel(user));
        }

        // -------------------------------------------------------------------------
        // --- DELETE /api/users/:id ---

        [HttpDelete("{id}")]
        [RequiresPermission(Permissions.DELETE_USERS)]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            var user = await database.GetById<UserModel>(id);
            if (user == null)
                return NotFound();

            database.Delete(user);
            await database.Commit();

            return NoContent();
        }
    }
}
