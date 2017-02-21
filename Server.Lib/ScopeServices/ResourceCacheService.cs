using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Extensions;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources;

namespace Server.Lib.ScopeServices
{
    class ResourceCacheService : IResourceCacheService
    {
        public ResourceCacheService()
        {
            // Create our collections.
            this.resources = new Dictionary<string, Resource>();
            this.resourceLocks = new Dictionary<string, AsyncLock>();
        }

        private readonly IDictionary<string, Resource> resources;
        private readonly IDictionary<string, AsyncLock> resourceLocks;

        public async Task<TResource> FetchAsync<TResource>(string cacheId, Func<CancellationToken, Task<TResource>> fetcher, CancellationToken cancellationToken) where TResource : Resource
        {
            // Build the cache key for the requested resource.
            var resourceType = typeof(TResource);
            var cacheKey = this.GetCacheKey(resourceType, cacheId);

            // Check if we already have a resource with the corresponding key.
            if (this.resources.TryGetValue(cacheKey, out Resource resource))
                return (TResource)resource;

            // If none was found, lock this key and try again.
            using (await this.GetResourceLock(cacheKey).LockAsync(cancellationToken))
            {
                if (this.resources.TryGetValue(cacheKey, out resource))
                    return (TResource)resource;

                // If none was found, use the fetcher to retrieve it.
                resource = await fetcher(cancellationToken);

                // If the resource is null, update the cache and return.
                if (resource == null)
                {
                    this.resources[cacheKey] = null;
                    return null;
                }

                // Otherwise, if we have other keys, check their values.
                var otherCacheKeys = resource.CacheIds
                    .Where(c => c != cacheId)
                    .Select(c => this.GetCacheKey(resourceType, c))
                    .ToList();

                if (otherCacheKeys.Count > 0)
                {
                    // Lock all of the other keys first.
                    var otherCacheKeyLockTasks = otherCacheKeys.Select(c => this.GetResourceLock(c).LockAsync(cancellationToken)).ToList();
                    await Task.WhenAll(otherCacheKeyLockTasks);
                    using (otherCacheKeyLockTasks.Select(t => t.Result).ToDisposable())
                    {
                        // Find an existing value.
                        Resource existingResource = null;
                        if (otherCacheKeys.Any(c => this.resources.TryGetValue(c, out existingResource)))
                            resource = existingResource;

                        // Update all the keys with the new value.
                        foreach (var otherCacheKey in otherCacheKeys)
                        {
                            this.resources[otherCacheKey] = resource;
                        }
                    }
                }

                // Update the cache and return.
                this.resources[cacheKey] = resource;
                return (TResource)resource;
            }
        }

        public async Task SaveAsync<TResource>(TResource resource, Func<CancellationToken, Task> saver, CancellationToken cancellationToken = new CancellationToken()) where TResource : Resource
        {
            // Build the cache keys for the specified resource.
            var resourceType = typeof(TResource);
            var cacheKeys = resource.CacheIds.Select(c => this.GetCacheKey(resourceType, c)).ToList();

            // Lock all the keys.
            var cacheKeyLockTasks = cacheKeys.Select(c => this.GetResourceLock(c).LockAsync(cancellationToken)).ToList();
            await Task.WhenAll(cacheKeyLockTasks);
            using (cacheKeyLockTasks.Select(t => t.Result).ToDisposable())
            {
                // Save our resource.
                await saver(cancellationToken);

                // The keys to update will depend on whether this is a versioned resource or not.
                var versionedResource = resource as VersionedResource;
                var cacheKeysToUpdate = versionedResource == null
                    ? cacheKeys
                    : cacheKeys.Where(c =>
                    {
                        // None was found, update.
                        var existingVersionedResource = this.resources.TryGetValue(c) as VersionedResource;
                        if (existingVersionedResource == null)
                            return true;

                        // Otherwise, compare the date and version Id.
                        return versionedResource.CompareTo(existingVersionedResource) >= 0;
                    }).ToList();

                // Finally, update the cache.
                foreach (var cacheKey in cacheKeysToUpdate)
                {
                    this.resources[cacheKey] = resource;
                }
            }
        }

        private AsyncLock GetResourceLock(string resourceKey)
        {
            // If we already have a lock for this key, return it.
            var resourceLock = this.resourceLocks.TryGetValue(resourceKey);
            if (resourceLock != null)
                return resourceLock;
            
            // Otherwise, lock and retry.
            lock (this.resourceLocks)
            {
                resourceLock = this.resourceLocks.TryGetValue(resourceKey);
                if (resourceLock != null)
                    return resourceLock;

                // If none was found, create it.
                resourceLock = new AsyncLock();
                this.resourceLocks[resourceKey] = resourceLock;

                return resourceLock;
            }
        }

        private string GetCacheKey(Type resourceType, string cacheId)
        {
            return $"{resourceType.Name}-{cacheId}";
        } 
    }
}