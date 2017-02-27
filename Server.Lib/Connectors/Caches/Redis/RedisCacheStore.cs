using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Helpers;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources.Cache;
using StackExchange.Redis;

namespace Server.Lib.Connectors.Caches.Redis
{
    public class RedisCacheStore<TCacheResource> : ICacheStore<TCacheResource> where TCacheResource : CacheResource
    {
        public RedisCacheStore(
            IJsonHelpers jsonHelpers,
            IConfiguration configuration,
            IDatabase db)
        {
            Ensure.Argument.IsNotNull(jsonHelpers, nameof(jsonHelpers));
            Ensure.Argument.IsNotNull(configuration, nameof(configuration));
            Ensure.Argument.IsNotNull(db, nameof(db));
            
            this.jsonHelpers = jsonHelpers;
            this.db = db;

            // Find the cache key for this resource type.
            var resourceType = typeof(TCacheResource);
            this.cacheKeyRoot = configuration.CachePrefixes[resourceType];
        }
        
        private readonly IJsonHelpers jsonHelpers;
        private readonly IDatabase db;

        private readonly string cacheKeyRoot;

        public async Task<Optional<TCacheResource>> GetAsync(string cacheId, CancellationToken cancellationToken)
        {
            // Retrieve the value from Redis.
            var cacheKey = this.GetCacheKey(cacheId);
            var stringValue = await this.db.StringGetAsync(cacheKey);
            if (!stringValue.HasValue)
                return new Optional<TCacheResource>();

            // If a value was found, but it's "null", return.
            if (stringValue == "null")
                return new Optional<TCacheResource>(null);

            // Deserialize and return.
            return new Optional<TCacheResource>(this.jsonHelpers.FromJsonString<TCacheResource>(stringValue));
        }

        public async Task SaveAsync(string[] cacheIds, TCacheResource cacheResource, CancellationToken cancellationToken)
        {
            // We'll update the various cache keys in a redis transaction.
            var transaction = this.db.CreateTransaction();
            string[] cacheIdsToSave;

            // If the value is null, make sure we don't overwrite anything.
            if (cacheResource == null)
            {
                foreach (var cacheId in cacheIds)
                {
                    transaction.AddCondition(Condition.KeyNotExists(this.GetCacheKey(cacheId)));
                }
            }

            // If this is not a versioned resource, save to all the provided Ids.
            var versionedCacheResource = cacheResource as CacheVersionedResource;
            if (versionedCacheResource == null)
            {
                cacheIdsToSave = cacheIds;
            }
            // Otherwise, check their values first.
            else
            {
                var fetchExistingValueTasks = cacheIds.ToDictionary(
                    c => c,
                    c => this.db.StringGetAsync(this.GetCacheKey(c))
                );
                await Task.WhenAll(fetchExistingValueTasks.Values);

                // Find the cache Ids that we actually want to update.
                cacheIdsToSave = fetchExistingValueTasks
                    .Where(kv =>
                    {
                        // If no string value was found, no conflict.
                        if (!kv.Value.Result.HasValue)
                            return true;

                        // Otherwise, deserialize and test.
                        var existingCacheResource = this.jsonHelpers.FromJsonString<CacheVersionedResource>(kv.Value.Result);
                        return versionedCacheResource.CompareTo(existingCacheResource) >= 0;
                    })
                    .Select(kv => kv.Key)
                    .ToArray();

                // Add conditions for the Ids that we're updating.
                foreach (var cacheId in cacheIdsToSave)
                {
                    var redisValue = fetchExistingValueTasks[cacheId];
                    transaction.AddCondition(redisValue.Result.HasValue
                        ? Condition.StringEqual(this.GetCacheKey(cacheId), redisValue.Result)
                        : Condition.KeyNotExists(this.GetCacheKey(cacheId)));
                }
            }

            // Add the operations to the transaction.
            var stringSetTasks = cacheIdsToSave.Select(c => 
                transaction.StringSetAsync(this.GetCacheKey(c), this.jsonHelpers.ToJsonString(cacheResource))
            ).ToList();

            // Run the transaction.
            await transaction.ExecuteAsync();
            await Task.WhenAll(stringSetTasks);
        }

        private string GetCacheKey(string cacheId)
        {
            return $"{this.cacheKeyRoot}/{cacheId}";
        }
    }
}