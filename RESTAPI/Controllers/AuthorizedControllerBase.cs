using DatabaseAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using RESTAPI.Services.Authorization;

namespace RESTAPI.Controllers
{
    public class AuthorizedControllerBase : ControllerBase, IAuthorizedController
    {
        public AuthClaims AuthClaims { get; set; }
        public UserModel AuthorizedUser { get; set; }
    }
}
