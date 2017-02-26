using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Connectors.Tables
{
    public interface ITables
    {
        IVersionedTable<CacheUser> Users { get; }
    }
}