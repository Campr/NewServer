using System;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Models.Resources;

namespace Server.Lib.ScopeServices
{
    public interface IVersionedResourceCacheService : IResourceCacheService
    {
        Task<TResource> FetchVersionAsync<TResource>(string id, string versionId, Func<Task<TResource>> fetcher, CancellationToken cancellationToken = default(CancellationToken)) where TResource : VersionedResource;
    }
}