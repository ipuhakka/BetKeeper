using System;
using System.Collections.Generic;
using System.Text;

namespace Betkeeper.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Convert a string to camel case
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string target)
        {
            return char.ToLowerInvariant(target[0]) + target[1..];
        }
    }
}
