using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Connectors.Db.Mongo
{
    public class MongoTable<TCacheResource> : ITable<TCacheResource> where TCacheResource : CacheResource
    {
        public MongoTable(IMongoCollection<TCacheResource> collection)
        {
            Ensure.Argument.IsNotNull(collection, nameof(collection));
            this.collection = collection;
        }

        private readonly IMongoCollection<TCacheResource> collection;

        public Task<TCacheResource> FindAsync(Expression<Func<TCacheResource, bool>> filter, CancellationToken cancellationToken)
        {
            return this.collection.Find(filter).FirstAsync(cancellationToken);
        }
    }
}