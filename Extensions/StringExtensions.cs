using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slms2asp.Extensions
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string str) =>
            str.Equals(default) || str.Length <= 0;
    }
}
