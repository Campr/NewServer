using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Connectors.Tables.Mongo
{
    public class VersionedMongoTable<TCacheResource> : MongoTable<TCacheResource>, IVersionedTable<TCacheResource> where TCacheResource : CacheVersionedResource
    {
        public VersionedMongoTable(IMongoCollection<TCacheResource> collection) : base(collection)
        {
        }

        public Task<TCacheResource> FindLastVersionAsync(Expression<Func<TCacheResource, bool>> filter, CancellationToken cancellationToken)
        {
            // Fetch only the last item matching our filter.
            return this.Collection
                .Find(filter)
                .SortByDescending(r => r.CreatedAt)
                .ThenByDescending(r => r.VersionId)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}