using DatabaseAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Gateway.Services.Authorization;

namespace Gateway.Controllers
{
    public class AuthorizedControllerBase : ControllerBase, IAuthorizedController
    {
        public AuthClaims AuthClaims { get; set; }
        public UserModel AuthorizedUser { get; set; }
    }
}
