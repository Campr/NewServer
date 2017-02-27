using System;
using Server.Lib.Models.Resources.Api;
using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Models.Resources
{
    public abstract class Resource<TCacheResource> where TCacheResource : CacheResource
    {
        public abstract string[][] CacheIds { get; }
        public abstract ApiResource ToApi();
        public abstract TCacheResource ToDb();

        public virtual TCacheResource ToCache()
        {
            return this.ToDb();
        }
        
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}