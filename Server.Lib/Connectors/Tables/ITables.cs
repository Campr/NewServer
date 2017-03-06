using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Connectors.Tables
{
    public interface ITables
    {
        ITable<TCacheResource> TableForType<TCacheResource>() where TCacheResource : CacheResource;
        IVersionedTable<TCacheResource> TableForVersionedType<TCacheResource>() where TCacheResource : VersionedCacheResource;
    }
}