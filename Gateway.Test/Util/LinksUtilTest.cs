using Gateway.Util;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gateway.Test.Util
{
    [TestFixture]
    class LinkUtilTest
    {
        private static readonly IEnumerable<string> ValidLinks = new[]
        {
            "https://google.com",
            "http://google.com",
            "https://www.google.com",
            "https://www.microsoft.com/en-us/",
            "https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-5.0",
            "https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core#build-web-apis-and-web-ui-using-aspnet-core-mvc"
        };

        private static readonly IEnumerable<string> InvalidLinks = new[]
        {
            "",
            "google.com",
            "htps://google.com",
            "https:/google.com",
            "https://google",
            "https://google/abc",
            "https://google.ahsdkjahsdkjahsdkjhasdkjhaksjdh",
            "https://.com",
            "https://google..com",
            "https://google.com/thispageis404",
        };

        [Test]
        public async Task ValidateDestinationTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                () => LinksUtil.ValidateDestination(null));

            foreach (var link in ValidLinks)
                Assert.IsTrue(
                    await LinksUtil.ValidateDestination(link),
                    $"Validation of (valid) link '{link}' failed");

            foreach (var link in InvalidLinks)
                Assert.IsFalse(
                    await LinksUtil.ValidateDestination(link),
                    $"Validation of (invalid) link '{link}' failed");
        }
    }
}
