using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Connectors.Db
{
    public interface ITables
    {
        ITable<CacheUser> Users { get; }
    }
}