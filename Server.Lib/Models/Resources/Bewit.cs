using System;
using Server.Lib.Models.Resources.Api;
using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Models.Resources
{
    public class Bewit : Resource
    {
        public byte[] Key { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public override string[] CacheIds => new [] { this.Id };

        public override ApiResource ToApi()
        {
            throw new NotImplementedException();
        }

        public override CacheResource ToCache()
        {
            throw new NotImplementedException();
        }
    }
}