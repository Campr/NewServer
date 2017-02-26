using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Server.Lib.Helpers
{
    public class TextHelpers : ITextHelpers
    {
        public string GenerateUniqueId()
        {
            return Guid.NewGuid().ToString("N");
        }

        public string BuildCacheKey(IEnumerable<string> cacheKeyParts)
        {
            return string.Join("/", cacheKeyParts.Select(WebUtility.UrlEncode));
        }

        public string ToJsonPropertyName(string propertyName)
        {
            // Make sure we have something to work with.
            if (string.IsNullOrWhiteSpace(propertyName))
                return propertyName;

            // Insert an underscore before all uppercase chars.
            propertyName = string.Concat(propertyName.Select((c, i) =>
                i > 0 && char.IsUpper(c) ? "_" + c.ToString() : c.ToString()));

            // Convert to lowercase and return.
            return propertyName.ToLowerInvariant();
        }
    }
}