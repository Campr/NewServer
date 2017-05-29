using System;

namespace Server.Lib.Helpers
{
    public interface ICryptoHelpers
    {
        string CreateBewit(DateTime expiresAt, Uri uri, string ext, string bewitId, byte[] key);
        string CreateMac(string header, DateTime timestamp, string nonce, string verb, Uri uri, string contentHash, string ext, string app, byte[] key);
        string CreateStaleTimestampMac(DateTime timestamp, byte[] key);
    }
}