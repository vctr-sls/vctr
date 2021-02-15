using DatabaseAccessLayer;
using DatabaseAccessLayer.Models;
using Gateway.Services;
using Gateway.Services.Hashing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gateway.Test.Services
{
    [TestFixture]
    class InitializationServiceTest
    {
        private const string username = "username";
        private const string password = "password";
        private const string hashed = "hashed:";

        [Test]
        public async Task InitializationRoutineTest()
        {
            var userModel = default(UserModel);

            var dbMock = new Mock<IDatabaseAccess>();
            dbMock.Setup(db => db.Create(It.IsAny<UserModel>()))
                  .Callback<UserModel>((u) => userModel = u);

            var hasherMock = new Mock<IPasswordHashingService>();
            hasherMock.Setup(h => h.GetEncodedHash(It.IsAny<string>()))
                      .ReturnsAsync((string s) => hashed + s);

            var loggerMock = new Mock<ILogger<InitializationService>>();

            var service = new InitializationService(dbMock.Object, GetConfiguration("", password), hasherMock.Object, loggerMock.Object);
            Assert.ThrowsAsync<ArgumentException>(() => service.InitializationRoutine());
            service = new InitializationService(dbMock.Object, GetConfiguration(username, ""), hasherMock.Object, loggerMock.Object);
            Assert.ThrowsAsync<ArgumentException>(() => service.InitializationRoutine());
            service = new InitializationService(dbMock.Object, GetConfiguration(null, password), hasherMock.Object, loggerMock.Object);
            Assert.ThrowsAsync<ArgumentException>(() => service.InitializationRoutine());
            service = new InitializationService(dbMock.Object, GetConfiguration(username, null), hasherMock.Object, loggerMock.Object);
            Assert.ThrowsAsync<ArgumentException>(() => service.InitializationRoutine());

            service = new InitializationService(dbMock.Object, GetConfiguration(), hasherMock.Object, loggerMock.Object);
            await service.InitializationRoutine();

            dbMock.Verify(db => db.Commit());
            Assert.AreEqual(username, userModel.UserName);
            Assert.AreEqual(hashed + password, userModel.PasswordHash);

            dbMock = new Mock<IDatabaseAccess>();
            dbMock.Setup(db => db.GetCount<UserModel>())
                  .ReturnsAsync(1)
                  .Verifiable();
            service = new InitializationService(dbMock.Object, GetConfiguration(), hasherMock.Object, loggerMock.Object);
            await service.InitializationRoutine();
            dbMock.Verify(db => db.GetCount<UserModel>());
            dbMock.VerifyNoOtherCalls();
        }

        private IConfiguration GetConfiguration(string un = username, string pw = password) =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>()
                    {
                        { Constants.ConfigKeyInitializationUserName, un },
                        { Constants.ConfigKeyInitializationPassword, pw }
                    })
                .Build();
    }
}
