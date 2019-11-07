using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using slms2asp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slms2asp.ActionResults
{
    public class ForbiddenActionResult : IActionResult
    {
        private string ErrorText;

        public ForbiddenActionResult(string errorText = null)
        {
            ErrorText = errorText;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var objectResult = new ObjectResult(ErrorModel.Forbidden(ErrorText))
            {
                StatusCode = StatusCodes.Status403Forbidden,
            };

            await objectResult.ExecuteResultAsync(context);
        }
    }
}
