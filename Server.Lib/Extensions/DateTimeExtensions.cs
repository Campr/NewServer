using System;

namespace Server.Lib.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CompareToMillisecond(this DateTime date1, DateTime date2)
        {
            return (int)Math.Floor((date1 - date2).TotalMilliseconds);
        }
    }
}