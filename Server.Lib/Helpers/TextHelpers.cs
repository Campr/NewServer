using System;
using System.Linq;
using System.Net;

namespace Server.Lib.Helpers
{
    class TextHelpers : ITextHelpers
    {
        public string GenerateUniqueId()
        {
            return Guid.NewGuid().ToString("N");
        }

        public string BuildCacheKey(string[] cacheKeyParts)
        {
            return string.Join("/", cacheKeyParts.Select(WebUtility.UrlEncode));
        }
    }
}