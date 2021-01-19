using System;
using System.Threading.Tasks;

namespace Gateway.Services.Hashing
{
    public abstract class Base64HashEncoder
    {
        public Task<bool> CompareEncodedHash(string password, string encodedHash) =>
            new ValueTask<bool>(GetBase64Hash(password) == encodedHash).AsTask();

        public Task<string> GetEncodedHash(string password) =>
            new ValueTask<string>(GetBase64Hash(password)).AsTask();

        private string GetBase64Hash(string data) => 
            Convert.ToBase64String(GetHash(data));

        protected abstract byte[] GetHash(string data);
    }
}
