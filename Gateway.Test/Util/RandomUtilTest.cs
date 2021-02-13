using Gateway.Util;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gateway.Test.Util
{
    [TestFixture]
    class RandomUtilTest
    {
        [Test]
        public void GetStringTest()
        {
            Assert.Throws<ArgumentException>(
                () => RandomUtil.GetString(0, "abc"));
            Assert.Throws<ArgumentException>(
                () => RandomUtil.GetString(20, ""));
            Assert.Throws<ArgumentException>(
                () => RandomUtil.GetString(20, null));

            var str = RandomUtil.GetString(18, "abc123");
            Assert.AreEqual(18, str.Length);
            Assert.IsTrue(new Regex(@"^[abc123]+$").IsMatch(str));

            var randSet = Enumerable.Range(0, 1000)
                .Select(_ => RandomUtil.GetString(16, "abcdefghijklmnopqrstuvwxyz123456789"));
            Assert.IsNull(randSet.FirstOrDefault(r => randSet.Count(ru => r == ru) > 1));
        }
    }
}
