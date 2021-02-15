using Gateway.Services.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Gateway.Test.Services.Authorization
{
    [TestFixture]
    class JwtAuthorizationServiceTest
    {
        private static readonly AuthClaims testClaims = new AuthClaims()
        {
            Guid = Guid.NewGuid()
        };

        [Test]
        public void GetSessionKeyTest()
        {
            var cfg = GetConfig();
            var auth = new JwtAuthorizationService(cfg);

            var sessionKey1 = auth.GetSessionKey(testClaims, TimeSpan.FromMinutes(10));
            var sessionKey2 = auth.GetSessionKey(testClaims, TimeSpan.FromMinutes(10));
            var sessionKey3 = auth.GetSessionKey(new AuthClaims(), TimeSpan.FromMinutes(10));
            Assert.AreEqual(sessionKey1, sessionKey2);
            Assert.AreNotEqual(sessionKey1, sessionKey3);

            auth = new JwtAuthorizationService(cfg);
            var sessionKey4 = auth.GetSessionKey(testClaims, TimeSpan.FromMinutes(10));
            Assert.AreNotEqual(sessionKey1, sessionKey4);

            cfg = GetConfig("testKeyWithALengthOfAtLeast32Bit");
            sessionKey1 = new JwtAuthorizationService(cfg)
                .GetSessionKey(testClaims, TimeSpan.FromMinutes(10));
            sessionKey2 = new JwtAuthorizationService(cfg)
                .GetSessionKey(testClaims, TimeSpan.FromMinutes(10));
            Assert.AreEqual(sessionKey1, sessionKey2);
        }

        [Test]
        public void ValidateSessionKeyTest()
        {
            var cfg = GetConfig();
            var auth = new JwtAuthorizationService(cfg);

            var sessionKey = auth.GetSessionKey(testClaims, TimeSpan.FromMilliseconds(250));
            var recoveredClaims = auth.ValidateSessionKey(sessionKey);
            Assert.AreEqual(testClaims.Guid, recoveredClaims.Guid);

            sessionKey = new JwtAuthorizationService(cfg)
                .GetSessionKey(testClaims, TimeSpan.FromMinutes(10));
            Assert.Throws<SecurityTokenInvalidSignatureException>(
                () => new JwtAuthorizationService(cfg).ValidateSessionKey(sessionKey));

            cfg = GetConfig("testKeyWithALengthOfAtLeast32Bit");
            sessionKey = new JwtAuthorizationService(cfg)
                .GetSessionKey(testClaims, TimeSpan.FromMinutes(10));
            recoveredClaims = new JwtAuthorizationService(cfg)
                .ValidateSessionKey(sessionKey);
            Assert.AreEqual(testClaims.Guid, recoveredClaims.Guid);
        }

        [Test]
        public void ValidateSessionKey_TimeoutTest()
        {
            var cfg = GetConfig();
            var auth = new JwtAuthorizationService(cfg);

            var sessionKey = auth.GetSessionKey(testClaims, TimeSpan.FromMilliseconds(250));

            Thread.Sleep(15000);
            Assert.Throws<SecurityTokenExpiredException>(
                () => auth.ValidateSessionKey(sessionKey));
        }

        private IConfiguration GetConfig(string key = null) =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> { { Constants.ConfigKeySessionsJwtSecret, key } })
                .Build();
    }
}
