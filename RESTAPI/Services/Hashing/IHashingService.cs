using System.Threading.Tasks;

namespace RESTAPI.Services.Hashing
{
    public interface IHashingService
    {
        Task<string> GetEncodedHash(string password);
        Task<bool> CompareEncodedHash(string password, string encodedHash);
    }
}
