using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Services.Hashing
{
    public class Sha1HashingService : IHashingService
    {
        public Task<bool> CompareEncodedHash(string password, string encodedHash) =>
            new ValueTask<bool>(GetBase64Hash(password) == encodedHash).AsTask();

        public Task<string> GetEncodedHash(string password) =>
            new ValueTask<string>(GetBase64Hash(password)).AsTask();

        private string GetBase64Hash(string data)
        {
            var bHash = SHA1.HashData(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(bHash);
        }
    }
}
