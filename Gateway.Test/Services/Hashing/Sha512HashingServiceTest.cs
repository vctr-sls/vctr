using Gateway.Services.Hashing;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Gateway.Test.Services.Hashing
{
    [TestFixture]
    class Sha512HashingServiceTest : HasherTestBase<Sha512HashingService>
    {
        [Test]
        public override Task CompareEncodedHashTest() =>
            base.CompareEncodedHashTest();

        [Test]
        public override Task GetEncodedHashTest() =>
            base.GetEncodedHashTest();

        public override Sha512HashingService GetHasherInstance(IConfiguration config = null) =>
            new Sha512HashingService();
    }
}
