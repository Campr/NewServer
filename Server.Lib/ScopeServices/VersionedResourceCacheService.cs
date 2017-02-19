using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Extensions;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources;

namespace Server.Lib.ScopeServices
{
    class VersionedResourceCacheService : IVersionedResourceCacheService
    {
        public VersionedResourceCacheService()
        {
            // Create the resource collections.
            this.resources = new Dictionary<string, Resource>();
            this.versionedResources = new Dictionary<string, VersionedResource>();

            // Create the lock collection.
            this.resourceLocks = new Dictionary<string, AsyncLock>();
        }

        private readonly IDictionary<string, Resource> resources;
        private readonly IDictionary<string, VersionedResource> versionedResources;

        private readonly IDictionary<string, AsyncLock> resourceLocks;

        public async Task<TResource> FetchAsync<TResource>(string id, Func<Task<TResource>> fetcher, CancellationToken cancellationToken) where TResource : Resource
        {
            // Build the resource key for the requested object.
            var resourceType = typeof(TResource);
            var resourceKey = this.GetResourceKey(resourceType, id);

            // Check if we already have a resource with the corresponding key.
            var resource = this.resources.TryGetValue(resourceKey);
            if (resource != null)
                return (TResource)resource;

            // If none was found, lock this key and try again.
            using (await this.GetResourceLock(resourceKey).LockAsync(cancellationToken))
            {
                resource = this.resources.TryGetValue(resourceKey);
                if (resource != null)
                    return (TResource)resource;

                // If none was found, use the fetcher to retrieve it.
                resource = await fetcher();

                // If this is a versioned resource, check the versioned cache.
                var versionedResource = resource as VersionedResource;
                if (versionedResource != null)
                {
                    var versionedResourceKey = this.GetVersionedResourceId(resourceType, versionedResource.Id, versionedResource.VersionId);
                    var cachedVersionedResource = this.versionedResources.TryGetValue(versionedResourceKey);

                    // If a versioned resource was found, use it instead of our newly fetched resource.
                    if (cachedVersionedResource != null)
                        resource = cachedVersionedResource;
                    // Otherwise, update the versioned resource cache.
                    else
                        this.versionedResources[versionedResourceKey] = versionedResource;
                }

                // Update the cache and return.
                this.resources[resourceKey] = resource;
                return (TResource)resource;
            }
        }

        public async Task<TResource> FetchVersionAsync<TResource>(string id, string versionId, Func<Task<TResource>> fetcher, CancellationToken cancellationToken) where TResource : VersionedResource
        {
            // Build the resource keys for the requested object.
            var resourceType = typeof(TResource);
            var resourceKey = this.GetResourceKey(resourceType, id);
            var versionedResourceKey = this.GetVersionedResourceId(resourceType, id, versionId);

            // Check if we already have a resource with the corresponding key.
            var versionedResource = this.versionedResources.TryGetValue(versionedResourceKey);
            if (versionedResource != null)
                return (TResource)versionedResource;

            // If none was found, lock the keys and try again.
            using (await this.GetResourceLock(resourceKey).LockAsync(cancellationToken))
            using (await this.GetResourceLock(versionedResourceKey).LockAsync(cancellationToken))
            {
                versionedResource = this.versionedResources.TryGetValue(versionedResourceKey);
                if (versionedResource != null)
                    return (TResource)versionedResource;

                // If we already have a resource with the same version, use it instead.
                var resource = this.resources.TryGetValue(resourceKey) as TResource;
                if (resource?.VersionId == versionId)
                    versionedResource = resource;

                // If none was found, use the fetcher to retrieve it.
                if (versionedResource == null)
                    versionedResource = await fetcher();

                // Update the cache and return.
                this.versionedResources[versionedResourceKey] = versionedResource;
                return (TResource)versionedResource;
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

        private string GetResourceKey(Type resourceType, string id)
        {
            return $"{resourceType.Name}-{id}";
        }

        private string GetVersionedResourceId(Type resourceType, string id, string versionId)
        {
            return $"{resourceType.Name}-{id}-{versionId}";
        }
    }
}