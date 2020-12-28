using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Betkeeper.Extensions
{
    public static class DictionaryExtensions
    {
        public static string GetString(this Dictionary<string, object> dictionary, string key)
        {
            if (key == null || !dictionary.ContainsKey(key))
            {
                return null;
            }

            return dictionary[key].ToString();
        }

        public static DateTime? GetDateTime(this Dictionary<string, object> dictionary, string key)
        {
            if (!dictionary.ContainsKey(key))
            {
                return null;
            }

            if (DateTime.TryParse(dictionary[key].ToString(), out DateTime dateTime))
            {
                return dateTime;
            }

            return null;
        }

        public static int? GetInt(this Dictionary<string, object> dictionary, string key)
        {
            if (key == null
                || !dictionary.ContainsKey(key)
                || dictionary[key] == null)
            {
                return null;
            }

            if (int.TryParse(dictionary[key].ToString(), out int i))
            {
                return i;
            }

            return null;
        }

        public static string GetKeyLike(this Dictionary<string, object> dictionary, string keyLike)
        {
            return dictionary
                .Keys
                .FirstOrDefault(key => key.Contains(keyLike));
        }

        /// <summary>
        /// Returns last part of key splitted with '-' character as integer.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="keyLike"></param>
        /// <returns></returns>
        public static int? GetIdentifierFromKeyLike(this Dictionary<string, object> dictionary, string keyLike)
        {
            if (string.IsNullOrEmpty(keyLike))
            {
                return null;
            }

            var keyMatch = dictionary.GetKeyLike(keyLike);

            if (keyMatch == null)
            {
                return null;
            }

            return int.TryParse(keyMatch.Split('-').Last(), out int parsed)
                ? parsed
                : (int?)null;
        }

        /// <summary>
        /// Returns a double from dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static double? GetDouble(this Dictionary<string, object> dictionary, string key)
        {
            var asString = dictionary.GetString(key);

            if (string.IsNullOrEmpty(asString))
            {
                return null;
            }

            asString = asString.Replace(",", ".");

            return double.TryParse(asString, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double result)
                ? result
                : (double?)null;
        }
    }
}
