using System;
using Server.Lib.Models.Resources.Api;
using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Models.Resources
{
    public abstract class Resource
    {
        public abstract string[][] CacheIds { get; }
        public abstract ApiResource ToApi();
        public abstract CacheResource ToDb();

        public virtual CacheResource ToCache()
        {
            return this.ToDb();
        }

        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}