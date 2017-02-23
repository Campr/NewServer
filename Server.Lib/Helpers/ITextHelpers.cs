namespace Server.Lib.Helpers
{
    public interface ITextHelpers
    {
        string GenerateUniqueId();

        string BuildCacheKey(string[] cacheKeyParts);
    }
}