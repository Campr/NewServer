using System.Collections.Generic;

namespace Server.Lib.Extensions
{
    public static class StringExtensions
    {
        public static string Join(this IEnumerable<string> strings, string separator)
        {
            return string.Join(separator, strings);
        }

        public static int? TryParseInt(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            int intValue;
            return int.TryParse(value, out intValue)
                ? intValue
                : (int?)null;
        }
    }
}