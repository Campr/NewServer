using MongoDB.Driver;
using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Connectors.Db.Mongo
{
    public class MongoTables : ITables
    {
        public MongoTables()
        {
            var client = new MongoClient("");
            var database = client.GetDatabase("");

            this.Users = new VersionedMongoTable<CacheUser>(database.GetCollection<CacheUser>(""));
        }

        public IVersionedTable<CacheUser> Users { get; }
    }
}