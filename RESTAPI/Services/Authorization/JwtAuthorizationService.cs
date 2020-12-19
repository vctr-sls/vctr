using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RESTAPI.Services.Authorization
{
    public class JwtAuthorizationService : IAuthorizationService
    {
        private static readonly string issuer = "voidsearch API";
        private static readonly int keyLen = 32;
        private static readonly string securityAlgorithm = SecurityAlgorithms.HmacSha256Signature;

        private SymmetricSecurityKey signingKey;
        private readonly JwtSecurityTokenHandler tokenHandler;

        public JwtAuthorizationService(IConfiguration config)
        {
            var secret = config.GetValue<string>(Constants.ConfigKeySessionsJwtSecret);

            signingKey = string.IsNullOrEmpty(secret)
                ? GenerateSigningKey(keyLen)
                : new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            tokenHandler = new JwtSecurityTokenHandler();
        }

        public string GetSessionKey(AuthClaims claims, TimeSpan expire)
        {
            var ci = new ClaimsIdentity();
            ci.AddClaim(new Claim(ClaimTypes.NameIdentifier, claims.Guid.ToString()));

            var credentials = new SigningCredentials(signingKey, securityAlgorithm);

            var token = tokenHandler.CreateJwtSecurityToken(
                  issuer: issuer,
                  subject: ci,
                  notBefore: DateTime.UtcNow,
                  expires: DateTime.UtcNow.Add(expire),
                  signingCredentials: credentials);

            return tokenHandler.WriteToken(token);
        }

        public AuthClaims ValidateSessionKey(string sessionKey)
        {
            var parameters = new TokenValidationParameters
            {
                IssuerSigningKey = signingKey,
                ValidIssuer = issuer,
                ValidateIssuer = true,
                ValidateAudience = false
            };

            var principal = tokenHandler.ValidateToken(sessionKey, parameters, out var _);
            var claims = principal.Identities.First().Claims;

            var userId = claims?.First(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new Exception("invalid claims");

            return new AuthClaims
            {
                Guid = Guid.Parse(userId),
            };
        }

        /// <summary>
        /// Generates a cryptographically random array
        /// of bytes with the length of len and returns
        /// it as SymmetricSecurityKey.
        /// </summary>
        /// <param name="len">key length</param>
        /// <returns>key</returns>
        private SymmetricSecurityKey GenerateSigningKey(int len)
        {
            var bytes = new byte[len];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(bytes);
            return new SymmetricSecurityKey(bytes);
        }
    }
}
