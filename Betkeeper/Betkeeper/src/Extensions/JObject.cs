using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Betkeeper.Extensions
{
    public static class JObjectExtensions
    {
        /// <summary>
        /// Gets first key from jobject which is like parameter. Returns null if key is not found
        /// </summary>
        /// <param name="jObject"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetKeyLike(this JObject jObject, string keyLike)
        {
            return jObject
                .Properties()
                .FirstOrDefault(property => property.Name.Contains(keyLike))
                ?.Name;
        }

        /// <summary>
        /// Returns list of keys which contain input parameter.
        /// </summary>
        /// <param name="jObject"></param>
        /// <param name="keyLike"></param>
        /// <returns></returns>
        public static List<string> GetKeysLike(this JObject jObject, string keyLike)
        {
            return jObject
                .Properties()
                .Where(property => property.Name.Contains(keyLike))
                .Select(property => property.Name)
                .ToList();
        }

        /// <summary>
        /// Converts last element in property name splitted by a '-' into an integer.
        /// </summary>
        /// <param name="jObject"></param>
        /// <param name="keyLike"></param>
        /// <returns></returns>
        public static int GetIdentifierValueFromKeyLike(this JObject jObject, string keyLike)
        {
            return int.Parse(
                GetKeyLike(jObject, keyLike)
                    .Split('-')
                    .Last());
        }
    }
}
