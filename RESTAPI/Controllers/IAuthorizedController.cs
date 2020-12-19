using DatabaseAccessLayer.Models;
using RESTAPI.Services.Authorization;

namespace RESTAPI.Controllers
{
    public interface IAuthorizedController
    {
        AuthClaims AuthClaims { get; set; }

        UserModel AuthorizedUser { get; set; }
    }
}
