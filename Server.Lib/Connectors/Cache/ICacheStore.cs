using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Models.Resources;

namespace Server.Lib.Connectors.Cache
{
    public interface ICacheStore<TResource> where TResource : Resource
    {
        Task<TResource> Get(string cacheId, CancellationToken cancellationToken = default(CancellationToken));
        Task Save(TResource resource, CancellationToken cancellationToken = default(CancellationToken));
    }
}