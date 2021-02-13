using System;
using System.Linq;

namespace Gateway.Util
{
    public static class RandomUtil
    {
        private static readonly Random rng = new Random(DateTime.Now.Millisecond);

        public static string GetString(int len, string chars)
        {
            if (len < 1) throw new ArgumentException("len must be larger than 0");
            if (!(chars?.Length >= 1)) throw new ArgumentException("invalid character set");

            return new string(Enumerable.Repeat(chars, len)
                .Select(c => c[rng.Next(c.Length)]).ToArray());
        }
    }
}
