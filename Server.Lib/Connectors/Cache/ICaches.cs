using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Connectors.Cache
{
    public interface ICaches
    {
        ICacheStore<CacheUser> Users { get; }
    }
}