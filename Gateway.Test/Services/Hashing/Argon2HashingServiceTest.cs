using Gateway.Services.Hashing;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Gateway.Test.Services.Hashing
{
    [TestFixture]
    class Argon2HashingServiceTest : HasherTestBase<Argon2HashingService>
    {
        [Test]
        public override Task GetEncodedHashTest() =>
            base.GetEncodedHashTest();

        [Test]
        public override Task CompareEncodedHashTest() =>
            base.CompareEncodedHashTest();
    }
}
