using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Connectors.Caches
{
    public interface ICaches : IConnector
    {
        ICacheStore<TCacheResource> StoreForType<TCacheResource>() where TCacheResource : CacheResource;
        ICacheStore<CacheUser> Users { get; }
    }
}