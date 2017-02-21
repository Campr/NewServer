using Server.Lib.Models.Resources;

namespace Server.Lib.Connectors.Cache
{
    public interface ICaches
    {
        ICacheStore<User> Users { get; }
        ICacheStore<Bewit> Bewits { get; }
    }
}