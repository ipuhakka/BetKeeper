using System;

namespace Betkeeper.Extensions
{
    public static class DateTimeExtensions
    {

        /// <summary>
        /// Trim's datetime to given unit.
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static DateTime Trim(this DateTime datetime, long ticks)
        {
            return new DateTime(
                datetime.Ticks - (datetime.Ticks % ticks),
                datetime.Kind);
        }

    }
}
