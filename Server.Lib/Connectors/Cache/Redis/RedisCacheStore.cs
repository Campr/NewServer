using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Helpers;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources;
using Server.Lib.Models.Resources.Cache;
using Server.Lib.Services;
using StackExchange.Redis;

namespace Server.Lib.Connectors.Cache.Redis
{
    public class RedisCacheStore<TResource> : ICacheStore<TResource> where TResource : Resource
    {
        public RedisCacheStore(
            IResourceLoader resourceLoader,
            IJsonHelpers jsonHelpers,
            IConfiguration configuration,
            IDatabase db)
        {
            Ensure.Argument.IsNotNull(resourceLoader, nameof(resourceLoader));
            Ensure.Argument.IsNotNull(jsonHelpers, nameof(jsonHelpers));
            Ensure.Argument.IsNotNull(configuration, nameof(configuration));
            Ensure.Argument.IsNotNull(db, nameof(db));

            this.resourceLoader = resourceLoader;
            this.jsonHelpers = jsonHelpers;
            this.db = db;

            // Find the cache key for this resource type.
            var resourceType = typeof(TResource);
            this.cacheKeyRoot = configuration.ResourceCacheKeys[resourceType];
        }

        private readonly IResourceLoader resourceLoader;
        private readonly IJsonHelpers jsonHelpers;
        private readonly IDatabase db;

        private readonly string cacheKeyRoot;

        public async Task<TResource> Get(string cacheId, CancellationToken cancellationToken)
        {
            // Retrieve the value from Redis.
            var cacheKey = this.GetCacheKey(cacheId);
            var stringValue = await this.db.StringGetAsync(cacheKey);
            if (!stringValue.HasValue)
                return null;

            // Deserialize and return.
            return await this.resourceLoader.LoadFromString<TResource>(stringValue);
        }

        public async Task Save(TResource resource, CancellationToken cancellationToken)
        {
            // Serialize the provided resource.
            var cacheResource = resource.ToCache();

            // We'll update the various cache keys in a redis transaction.
            var transaction = this.db.CreateTransaction();
            string[] cacheIds;

            // If this is not a versioned resource, save to all the provided Ids.
            var versionedCacheResource = cacheResource as VersionedCacheResource;
            if (versionedCacheResource == null)
            {
                cacheIds = resource.CacheIds;
            }
            // Otherwise, check their values first.
            else
            {
                var fetchExistingValueTasks = resource.CacheIds.ToDictionary(
                    c => c,
                    c => this.db.StringGetAsync(this.GetCacheKey(c))
                );
                await Task.WhenAll(fetchExistingValueTasks.Values);
                
                // Find the cache Ids that we actually want to update.
                cacheIds = fetchExistingValueTasks
                    .Where(kv =>
                    {
                        // If no string value was found, no conflict.
                        if (!kv.Value.Result.HasValue)
                            return true;

                        // Otherwise, deserialize and test.
                        var existingCacheResource = this.jsonHelpers.FromJsonString<VersionedCacheResource>(kv.Value.Result);
                        return versionedCacheResource.CompareTo(existingCacheResource) >= 0;
                    })
                    .Select(kv => kv.Key)
                    .ToArray();

                // Add conditions for the Ids that we're updating.
                foreach (var cacheId in cacheIds)
                {
                    var redisValue = fetchExistingValueTasks[cacheId];
                    transaction.AddCondition(redisValue.Result.HasValue
                        ? Condition.StringEqual(cacheId, redisValue.Result)
                        : Condition.KeyNotExists(cacheId));
                }
            }

            // Add the operations to the transaction.
            var stringSetTasks = cacheIds.Select(c => 
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