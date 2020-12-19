using DatabaseAccessLayer.Models;
using Gateway.Services.Authorization;

namespace Gateway.Controllers
{
    public interface IAuthorizedController
    {
        AuthClaims AuthClaims { get; set; }

        UserModel AuthorizedUser { get; set; }
    }
}
