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
    }
}