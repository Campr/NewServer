using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Models.Resources.Api;
using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Models.Resources
{
    public abstract class Resource<TCacheResource> where TCacheResource : CacheResource
    {
        public abstract ApiResource ToApi();
        public abstract TCacheResource ToCache();
        public abstract Task SaveAsync(CancellationToken cancellationToken = default(CancellationToken));

        protected virtual string[][] AllCacheIds => new[]
        {
            new [] { "id", this.Id }
        };

        public string[][] CacheIds => this.AllCacheIds
            .Where(c => !c.Any(string.IsNullOrWhiteSpace))
            .ToArray();
        
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}