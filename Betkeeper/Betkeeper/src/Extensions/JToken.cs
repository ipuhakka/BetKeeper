using System;
using Newtonsoft.Json.Linq;

namespace Betkeeper.Extensions
{
    public static class JTokenExtensions
    {
        /// <summary>
        /// Checks if JToken is null or whitespace.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this JToken token)
        {
            return string.IsNullOrWhiteSpace(token?.ToString());
        }
    }
}
