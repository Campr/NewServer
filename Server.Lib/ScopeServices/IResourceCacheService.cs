using System;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Models.Resources;

namespace Server.Lib.ScopeServices
{
    public interface IResourceCacheService
    {
        Task<TResource> FetchAsync<TResource>(string cacheId, Func<CancellationToken, Task<TResource>> fetcher, CancellationToken cancellationToken = default(CancellationToken)) where TResource : Resource;
        Task SaveAsync<TResource>(TResource resource, Func<CancellationToken, Task> saver, CancellationToken cancellationToken = default(CancellationToken)) where TResource : Resource;
    }
}