using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Connectors.Caches
{
    public interface ICacheStore<TCacheResource> where TCacheResource : CacheResource
    {
        Task<Optional<TCacheResource>> GetAsync(string cacheId, CancellationToken cancellationToken = default(CancellationToken));
        Task SaveAsync(string[] cacheIds, TCacheResource cacheResource, CancellationToken cancellationToken = default(CancellationToken));
    }
}