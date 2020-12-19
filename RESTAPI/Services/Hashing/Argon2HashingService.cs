using Konscious.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RESTAPI.Services.Hashing
{
    class HashingPreferences
    {
        private const int ARGON2ID_VERSION = 19;

        public int MemorySize { get; set; }
        public int DegreeOfParallelism { get; set; }
        public int Iterations { get; set; }
        public int SaltLength { get; set; }
        public int KeyLength { get; set; }

        public Argon2id MakeUnsaltedHasher(string password)
        {
            var argon2Id = new Argon2id(Encoding.UTF8.GetBytes(password));
            argon2Id.DegreeOfParallelism = DegreeOfParallelism;
            argon2Id.MemorySize = MemorySize;
            argon2Id.Iterations = Iterations;
            return argon2Id;
        }

        public string EncodeHash(byte[] hash, byte[] salt)
        {
            var b64Hash = Convert.ToBase64String(hash);
            var b64Salt = Convert.ToBase64String(salt);

            return $"$argon2id$v={ARGON2ID_VERSION}$m={MemorySize},t={Iterations},p={DegreeOfParallelism}${b64Salt}${b64Hash}";
        }

        public static (HashingPreferences Prefs, byte[] Salt, byte[] Hash) DecodeHash(string encodedHash)
        {
            if (!encodedHash.StartsWith("$argon2id")) throw new Exception("not an argon2id encoded hash");
            var split = encodedHash.Split("$");
            if (split.Length != 6) throw new Exception("invalid hash format");
            if (GetKvValue("v", split[2]) != ARGON2ID_VERSION) throw new Exception("invalid hash version");

            var prefs = new HashingPreferences();
            var paramsSplit = split[3].Split(",");
            prefs.MemorySize = GetKvValue("m", paramsSplit[0]);
            prefs.Iterations = GetKvValue("t", paramsSplit[1]);
            prefs.DegreeOfParallelism = GetKvValue("p", paramsSplit[2]);

            var salt = Convert.FromBase64String(split[4]);
            prefs.SaltLength = salt.Length;
            var hash = Convert.FromBase64String(split[5]);
            prefs.KeyLength = hash.Length;

            return (prefs, salt, hash);
        }

        private static int GetKvValue(string key, string kv)
        {
            var split = kv.Split("=");
            if (split.Length != 2 || split[0] != key) throw new Exception("invalid hash format");
            return int.Parse(split[1]);
        } 
    }

    public class Argon2HashingService : IPasswordHashingService
    {
        private static readonly RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

        private readonly HashingPreferences defaultPrefs = new HashingPreferences();

        public Argon2HashingService(IConfiguration config)
        {
            defaultPrefs.MemorySize = config.GetValue(Constants.ConfigKeyPasswordHashingMemoryPoolKB, 128 * 1024);
            defaultPrefs.DegreeOfParallelism = config.GetValue(Constants.ConfigKeyPasswordHashingDegreeOfParallelism, Environment.ProcessorCount);
            defaultPrefs.Iterations = config.GetValue(Constants.ConfigKeyPasswordHashingIterations, 4);
            defaultPrefs.SaltLength = config.GetValue(Constants.ConfigKeyPasswordHashingSaltLength, 16);
            defaultPrefs.KeyLength = config.GetValue(Constants.ConfigKeyPasswordHashingKeyLength, 32);
        }

        public async Task<string> GetEncodedHash(string password)
        {
            var argon2Id = defaultPrefs.MakeUnsaltedHasher(password);
            argon2Id.Salt = GetSalt(defaultPrefs.SaltLength);
            var hash = await argon2Id.GetBytesAsync(defaultPrefs.KeyLength);

            return defaultPrefs.EncodeHash(hash, argon2Id.Salt);
        }

        public async Task<bool> CompareEncodedHash(string password, string encodedHash)
        {
            var (prefs, salt, hash) = HashingPreferences.DecodeHash(encodedHash);

            var hasher = prefs.MakeUnsaltedHasher(password);
            hasher.Salt = salt;

            var compHash = await hasher.GetBytesAsync(prefs.KeyLength);
            return compHash.SequenceEqual(hash);
        }

        private byte[] GetSalt(int len)
        {
            var salt = new byte[len];
            rngCsp.GetBytes(salt);
            return salt;
        }
    }
}
