using System;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Models.Resources;
using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.ScopeServices
{
    public interface IResourceCacheService
    {
        Task<TResource> WrapFetchAsync<TResource, TCacheResource>(
            string cacheId,
            Func<CancellationToken, Task<TCacheResource>> fetcher,
            Func<TCacheResource, CancellationToken, Task<TResource>> loader,
            CancellationToken cancellationToken = default(CancellationToken))
                where TResource : Resource<TCacheResource>
                where TCacheResource : CacheResource;

        Task SaveAsync<TResource, TCacheResource>(
            TResource resource, 
            CancellationToken cancellationToken = default(CancellationToken)) 
                where TResource : Resource<TCacheResource>
                where TCacheResource : CacheResource;
    }
}