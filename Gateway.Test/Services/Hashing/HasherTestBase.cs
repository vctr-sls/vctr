using Gateway.Services.Hashing;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gateway.Test.Services.Hashing
{
    abstract class HasherTestBase<T> where T : IHashingService
    {
        private const string password = "thisIsAPassword";

        private readonly bool equalHashes;

        public HasherTestBase()
        {
            equalHashes = !typeof(IPasswordHashingService).IsAssignableFrom(typeof(T));
        }

        public virtual async Task GetEncodedHashTest()
        {
            var hasher = GetHasherInstance();

            Assert.ThrowsAsync<ArgumentNullException>(
                () => hasher.GetEncodedHash(null));
            Assert.ThrowsAsync<ArgumentException>(
                () => hasher.GetEncodedHash(""));

            var hash1 = await hasher.GetEncodedHash(password);
            var hash2 = await hasher.GetEncodedHash(password);

            if (equalHashes)
                Assert.AreEqual(hash1, hash2);
            else
                Assert.AreNotEqual(hash1, hash2);
        }

        public virtual async Task CompareEncodedHashTest()
        {
            var hasher = GetHasherInstance();

            Assert.ThrowsAsync<ArgumentNullException>(
                () => hasher.CompareEncodedHash(null, "hash"));
            Assert.ThrowsAsync<ArgumentException>(
                () => hasher.CompareEncodedHash("", "hash"));
            Assert.ThrowsAsync<ArgumentNullException>(
                () => hasher.CompareEncodedHash("pw", null));
            Assert.ThrowsAsync<ArgumentException>(
                () => hasher.CompareEncodedHash("pw", ""));

            var validHash = await hasher.GetEncodedHash(password);
            var invalidHash = await hasher.GetEncodedHash("invalidPassword");
            Assert.IsTrue(await hasher.CompareEncodedHash(password, validHash));
            Assert.IsFalse(await hasher.CompareEncodedHash(password, invalidHash));
        }

        public virtual T GetHasherInstance(IConfiguration config = null) =>
            (T)Activator.CreateInstance(typeof(T), config ?? GetConfig());

        private IConfiguration GetConfig(Dictionary<string, string> vals = null) =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(vals ?? new Dictionary<string, string>())
                .Build();
    }
}
