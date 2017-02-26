using System.Collections.Generic;

namespace Server.Lib.Helpers
{
    public interface ITextHelpers
    {
        string GenerateUniqueId();
        string BuildCacheKey(IEnumerable<string> cacheKeyParts);
        string ToJsonPropertyName(string propertyName);
    }
}