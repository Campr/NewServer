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

            this.Users = new MongoTable<CacheUser>(database.GetCollection<CacheUser>(""));
        }

        public ITable<CacheUser> Users { get; }
    }
}