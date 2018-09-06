using System;

namespace API
{
    public class Conversions
    {
        public static bool? ToNullableBool(string s)
        {
            if (s == null || s.Equals("null"))
                return null;
            return Convert.ToBoolean(s);
        }
    }
}
