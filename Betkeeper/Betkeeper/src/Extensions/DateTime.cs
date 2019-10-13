using System;

namespace Betkeeper.Extensions
{
    public static class DateTimeExtensions
    {

        public static DateTime Trim(this DateTime datetime, long ticks)
        {
            return new DateTime(
                datetime.Ticks - (datetime.Ticks % ticks),
                datetime.Kind);
        }

    }
}
