using System;
using System.Threading.Tasks;

namespace Gateway.Services.Hashing
{
    public abstract class Base64HashEncoder : IHashingService
    {
        public Task<bool> CompareEncodedHash(string password, string encodedHash)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (password.Length == 0) throw new ArgumentException("invalid length", nameof(password));
            if (encodedHash == null) throw new ArgumentNullException(nameof(encodedHash));
            if (encodedHash.Length == 0) throw new ArgumentException("invalid length", nameof(encodedHash));

            return new ValueTask<bool>(GetBase64Hash(password) == encodedHash).AsTask();
        }

        public Task<string> GetEncodedHash(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (password.Length == 0) throw new ArgumentException("invalid length", nameof(password));

            return new ValueTask<string>(GetBase64Hash(password)).AsTask();
        }

        private string GetBase64Hash(string data) => 
            Convert.ToBase64String(GetHash(data));

        protected abstract byte[] GetHash(string data);
    }
}
