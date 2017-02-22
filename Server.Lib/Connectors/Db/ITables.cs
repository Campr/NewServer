using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Connectors.Db
{
    public interface ITables
    {
        IVersionedTable<CacheUser> Users { get; }
    }
}