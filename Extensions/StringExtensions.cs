using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slms2asp.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 
        /// Returns if the string is null or the character
        /// length equals 0.
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns>is empty state</returns>
        public static bool IsEmpty(this string str) =>
            str.Equals(default) || str.Length <= 0;
    }
}
