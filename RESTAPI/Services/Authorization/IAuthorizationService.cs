using System;

namespace RESTAPI.Services.Authorization
{
    public interface IAuthorizationService
    {
        string GetSessionKey(AuthClaims claims, TimeSpan expire);

        AuthClaims ValidateSessionKey(string sessionKey);
    }
}
