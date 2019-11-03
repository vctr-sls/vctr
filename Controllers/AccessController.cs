using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using slms2asp.Database;
using slms2asp.Shared;

namespace slms2asp.Controllers
{
    // ----------------------------------------------------
    // -- Access Controller
    // -- /{shortIdent}
    // --
    // -- The actual controller for accesses of short links
    // -- which controlls the redirection flow.

    [Route("{shortIdent}")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly AppDbContext Db;

        public AccessController(AppDbContext _db)
        {
            Db = _db;
        }

        public async Task<IActionResult> Get(string shortIdent, [FromQuery] bool disableTracking)
        {
            var shortLink = Db.ShortLinks.SingleOrDefault(sl => sl.ShortIdent == shortIdent);

            if (shortLink == null)
            {
                return Content(ErrorResponseContent.SHORT_LINK_NOT_FOUND, MediaTypeNames.Text.Plain);
            }

            if (!shortLink.IsActive || 
                shortLink.AccessCount >= shortLink.MaxUses ||
                shortLink.Activates.CompareTo(DateTime.Now) < 0 ||
                shortLink.Expires.CompareTo(DateTime.Now) >= 0)
            {
                return Content(ErrorResponseContent.SHORT_LINK_NOT_FOUND, MediaTypeNames.Text.Plain);
            }

            // TODO: Actual access and tracking flow.

            return Redirect(shortLink.RootURL);
        }
    }
}