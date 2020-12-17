using System.Threading.Tasks;

namespace RESTAPI.Services
{
    public interface IHashingService
    {
        Task<string> GetEncodedHash(string password);
        Task<bool> CompareEncodedHash(string password, string encodedHash);
    }
}
