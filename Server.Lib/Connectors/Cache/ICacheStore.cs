using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Connectors.Cache
{
    public interface ICacheStore<TCacheResource> where TCacheResource : CacheResource
    {
        Task<TCacheResource> Get(string cacheId, CancellationToken cancellationToken = default(CancellationToken));
        Task Save(string[] cacheIds, TCacheResource cacheResource, CancellationToken cancellationToken = default(CancellationToken));
    }
}