using System;

namespace Server.Lib.Models.Resources.Cache
{
    public class CacheUser : VersionedCacheResource
    {
        public string Handle { get; set; }
        public string Entity { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool? IsBotFollowed { get; set; }
        public DateTime? LastDiscoveryAttempt { get; set; }
    }
}