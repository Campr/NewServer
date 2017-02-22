using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Connectors.Cache
{
    public interface ICaches
    {
        ICacheStore<TCacheResource> MakeForType<TCacheResource>() where TCacheResource : CacheResource;
        ICacheStore<CacheUser> Users { get; }
    }
}