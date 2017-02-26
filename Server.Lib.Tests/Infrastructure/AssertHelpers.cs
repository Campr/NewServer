using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Server.Lib.Tests.Infrastructure
{
    public static class AssertHelpers
    {
        public static void HasEqualFieldValues<T>(T expected, T actual)
        {
            // If both objects are null, no need to do anything.
            if (expected == null && actual == null)
                return;

            // At this point, we don't want any null value.
            Assert.NotNull(expected);
            Assert.NotNull(actual);

            var targetType = typeof(T);

            // List all the members that we're interested in.
            var fields = targetType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var failures = from field in fields
                let value1 = field.GetValue(expected)
                let value2 = field.GetValue(actual)
                where value1 != null || value2 != null
                where value1 == null || !AreEqual(value1, value2)
                select $"Field {field.Name}: Expected:<{value1}> Actual:<{value2}>";

            // Compare properties.
            var properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            failures = failures.Concat(from property in properties
                let value1 = property.GetValue(expected)
                let value2 = property.GetValue(actual)
                where value1 != null || value2 != null
                where value1 == null || !AreEqual(value1, value2)
                select $"Property {property.Name}: Expected:<{value1}> Actual:<{value2}>");
            
            // If no errors were found, return.
            var failuresList = failures.ToList();
            if (!failuresList.Any())
                return;
            
            Assert.True(false, "AssertHelpers.HasEqualFieldValues failed. "
                + Environment.NewLine + 
                string.Join(Environment.NewLine, failuresList));
        }

        private static bool AreEqual<T>(T expected, T actual)
        {
            // If this is a DateTime, compare with millisecond precision.
            if (expected is DateTime)
                return (long)Math.Floor(((DateTime)(object)expected - (DateTime)(object)actual).TotalMilliseconds) == 0;

            // Otherwise, use the default comparer.
            return expected.Equals(actual);
        }
    }
}