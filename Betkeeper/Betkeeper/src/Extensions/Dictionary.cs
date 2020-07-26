using System;
using System.Collections.Generic;

namespace Betkeeper.Extensions
{
    public static class DictionaryExtensions
    {
        public static string GetString(this Dictionary<string, object> dictionary, string key)
        {
            if (!dictionary.ContainsKey(key))
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

            DateTime dateTime;

            if (DateTime.TryParse(dictionary[key].ToString(), out dateTime))
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
    }
}
