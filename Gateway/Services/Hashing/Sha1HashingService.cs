﻿using System.Security.Cryptography;
using System.Text;

namespace Gateway.Services.Hashing
{
    public class Sha1HashingService : Base64HashEncoder, IHashingService
    {
        protected override byte[] GetHash(string data) =>
            SHA1.HashData(Encoding.UTF8.GetBytes(data));
    }
}
