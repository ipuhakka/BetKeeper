using System;
using System.Data;
using System.Globalization;

namespace Betkeeper.Extensions
{
    public static class DataRowExtensions
    {

        public static Enums.BetResult ToBetResult(this DataRow dataRow, string fieldKey)
        {
            var asInt = int.Parse(dataRow[fieldKey].ToString());

            if (asInt < -1 || asInt > 1)
            {
                throw new Exception(
                    string.Format("Could not convert {0} to BetResult", asInt));
            }

            return (Enums.BetResult)asInt;
        }

        public static double ToDouble(this DataRow dataRow, string fieldKey)
        {
            var asString = dataRow[fieldKey].ToString();

            asString = asString.Replace(",", ".");

            CultureInfo culture = new CultureInfo("us");
            return double.Parse(asString, culture);
        }

        public static int ToInt32(this DataRow dataRow, string fieldKey)
        {
            var asString = dataRow[fieldKey].ToString();

            if (int.TryParse(asString, out int result))
            {
                return result;
            }

            throw new Exception(
                string.Format(
                    "Integer conversion with input {0} failed",
                    dataRow[fieldKey].ToString()));
        }

        public static DateTime ToDateTime(this DataRow dataRow, string fieldKey)
        {
            if (DateTime.TryParse(
                dataRow[fieldKey].ToString(),
                out DateTime result))
            {
                return result;
            }

            throw new Exception(
                string.Format(
                    "Cannot convert {0} to DateTime",
                    dataRow[fieldKey].ToString()));
        }
    }
}
