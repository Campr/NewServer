using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Connectors.Caches
{
    public interface ICaches : IConnector
    {
        ICacheStore<TCacheResource> MakeForType<TCacheResource>() where TCacheResource : CacheResource;
        ICacheStore<CacheUser> Users { get; }
    }
}