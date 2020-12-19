using System;

namespace Gateway.Services.Authorization
{
    public interface IAuthorizationService
    {
        string GetSessionKey(AuthClaims claims, TimeSpan expire);

        AuthClaims ValidateSessionKey(string sessionKey);
    }
}
