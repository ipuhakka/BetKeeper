using System;
using System.Globalization;
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

        /// <summary>
        /// returns a JToken value as double, or null if conversion fails.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static double? GetDoubleInvariantCulture(this JToken token)
        {
            var asString = token.ToString().Trim();

            asString = asString.Replace(",", ".");

            CultureInfo culture = new CultureInfo("en-US");
            return double.TryParse(asString, NumberStyles.AllowDecimalPoint, culture, out double result)
                ? result
                : (double?)null;
        }
    }
}
