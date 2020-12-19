using System;
using System.Linq;

namespace RESTAPI.Util
{
    public static class RandomUtil
    {
        public static string GetString(int len, string chars)
        {
            Random rng = new Random(DateTime.Now.Millisecond);
            return new string(Enumerable.Repeat(chars, len)
                .Select(c => c[rng.Next(c.Length)]).ToArray());
        }
    }
}
