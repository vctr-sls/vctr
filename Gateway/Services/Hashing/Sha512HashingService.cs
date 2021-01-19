using System.Security.Cryptography;
using System.Text;

namespace Gateway.Services.Hashing
{
    public class Sha512HashingService : Base64HashEncoder, IApiKeyHashingService
    {
        protected override byte[] GetHash(string data) =>
            SHA512.HashData(Encoding.UTF8.GetBytes(data));
    }
}
