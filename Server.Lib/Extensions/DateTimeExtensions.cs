﻿using System;

namespace Server.Lib.Extensions
{
    public static class DateTimeExtensions
    {
        private const string InvalidUnixEpochErrorMessage = "Unix epoc starts January 1st, 1970";

        /// <summary>
        /// Convert a millisecond long into a DateTime.
        /// </summary>
        public static DateTime FromUnixTime(this long self)
        {
            var ret = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return ret.AddMilliseconds(self);
        }

        /// <summary>
        /// Convert a second long into a DateTime.
        /// </summary>
        public static DateTime FromSecondTime(this long self)
        {
            var ret = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return ret.AddSeconds(self);
        }

        /// <summary>
        /// Convert a DateTime into a millisecond long.
        /// </summary>
        public static long ToUnixTime(this DateTime self)
        {
            if (self == DateTime.MinValue)
                return 0;

            var epoc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var delta = self - epoc;

            if (delta.TotalMilliseconds < 0) throw new ArgumentOutOfRangeException(InvalidUnixEpochErrorMessage);

            return (long)delta.TotalMilliseconds;
        }

        /// <summary>
        /// Convert a DateTime into a second long.
        /// </summary>
        public static long ToSecondTime(this DateTime self)
        {
            if (self == DateTime.MinValue)
                return 0;

            var epoc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var delta = self - epoc;

            if (delta.TotalSeconds < 0) throw new ArgumentOutOfRangeException(InvalidUnixEpochErrorMessage);

            return (long)delta.TotalSeconds;
        }

        /// <summary>
        /// Round DateTime to the lower Millisecond bound.
        /// </summary>
        /// <param name="self">The DateTime to truncate.</param>
        /// <returns>The truncated DateTime/</returns>
        public static DateTime TruncateToMilliseconds(this DateTime self)
        {
            return self.AddTicks(-(self.Ticks % TimeSpan.TicksPerMillisecond));
        }

        /// <summary>
        /// Compare two dates with a millisecond precision.
        /// </summary>
        /// <param name="date1">The first date to compare.</param>
        /// <param name="date2">The second date to compare.</param>
        /// <returns>A 32-bit signed integer that indicates whether <param name="date1"/> precedes, follows, or appears in the same position in the sort order as <param name="date2"/>.</returns>
        public static int CompareToMillisecond(this DateTime date1, DateTime date2)
        {
            return (int)Math.Floor((date1 - date2).TotalMilliseconds);
        }
    }
}