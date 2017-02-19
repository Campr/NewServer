using System;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Models.Resources;

namespace Server.Lib.ScopeServices
{
    public interface IResourceCacheService
    {
        Task<TResource> FetchAsync<TResource>(string id, Func<Task<TResource>> fetcher, CancellationToken cancellationToken = default(CancellationToken)) where TResource : Resource;
    }
}