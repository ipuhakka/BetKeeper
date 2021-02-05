using System;

namespace Betkeeper.Classes
{
    public static class EnumHelper
    {

        /// <summary>
        /// Parses enum from string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static T FromString<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase: true);
        }
    }
}
