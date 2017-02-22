using System;

namespace Server.Lib.Models.Resources.Cache
{
    public abstract class CacheResource
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}