using DatabaseAccessLayer;
using DatabaseAccessLayer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RESTAPI.Services.Hashing;
using System;
using System.Threading.Tasks;

namespace RESTAPI.Services
{
    public class InitializationService
    {
        private readonly IDatabaseAccess database;
        private readonly IConfiguration config;
        private readonly IPasswordHashingService pwHasher;
        private readonly ILogger<InitializationService> logger;

        public InitializationService(
            IDatabaseAccess _database, 
            IConfiguration _config, 
            IPasswordHashingService _pwHasher,
            ILogger<InitializationService> _logger)
        {
            database = _database;
            config = _config;
            pwHasher = _pwHasher;
            logger = _logger;
        }

        public async Task InitializationRoutine()
        {
            if (!(await IsInitialized()))
                await InitializeRootUser();
        }

        private async Task<bool> IsInitialized() =>
            await database.GetCount<UserModel>() > 0;

        private async Task InitializeRootUser()
        {
            logger.LogInformation("Initializing root user...");

            var userName = config.GetValue<string>(Constants.ConfigKeyInitializationUserName);
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException("root username not provided");

            var password = config.GetValue<string>(Constants.ConfigKeyInitializationPassword);
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("root user password not provided");

            var user = new UserModel
            {
                UserName = userName,
                PasswordHash = await pwHasher.GetEncodedHash(password),
                Permissions = Permissions.ADMINISTRATOR,
            };

            database.Create(user);
            await database.Commit();

            logger.LogInformation($"Root user with name '{userName}' and the provided password was created.");
        }
    }
}
