using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Connectors.Cache;
using Server.Lib.Connectors.Db;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources.Cache;
using Server.Lib.ScopeServices;

namespace Server.Lib.Models.Resources.Factories
{
    class UserFactory : IUserFactory
    {
        public UserFactory(
            IResourceCacheService resourceCacheService, 
            ICacheStore<CacheUser> usersCache, 
            ITable<CacheUser> usersTable)
        {
            Ensure.Argument.IsNotNull(resourceCacheService, nameof(resourceCacheService));
            Ensure.Argument.IsNotNull(usersCache, nameof(usersCache));
            Ensure.Argument.IsNotNull(usersTable, nameof(usersTable));

            this.resourceCacheService = resourceCacheService;
            this.usersCache = usersCache;
            this.usersTable = usersTable;
        }

        private readonly IResourceCacheService resourceCacheService;
        private readonly ICacheStore<CacheUser> usersCache;
        private readonly ITable<CacheUser> usersTable;

        public User MakeNew()
        {
            throw new System.NotImplementedException();
        }

        public Task<User> FetchAsync(string userId, CancellationToken cancellationToken = new CancellationToken())
        {
            return this.FetchByFilterAsync($"id/{userId}", u => u.Id == userId, cancellationToken);
        }

        public Task<User> FetchByEntityAsync(string entity, CancellationToken cancellationToken = new CancellationToken())
        {
            return this.FetchByFilterAsync($"entity/{entity}", u => u.Entity == entity, cancellationToken);
        }

        public Task<User> FetchByEmailAsync(string email, CancellationToken cancellationToken = new CancellationToken())
        {
            return this.FetchByFilterAsync($"email/{email}", u => u.Email == email, cancellationToken);
        }

        private Task<User> FetchByFilterAsync(string cacheId, Expression<Func<CacheUser, bool>> filter, CancellationToken cancellationToken)
        {
            // We may already have fetched this User.
            return this.resourceCacheService.WrapFetchAsync(cacheId, async ct =>
            {
                // Try to fetch from cache first.
                var cacheUser = await this.usersCache.Get(cacheId, cancellationToken);

                // If one was found, no need to continue.
                if (cacheUser != null)
                    return new User(cacheUser);

                // Otherwise, try from the Db.
                cacheUser = await this.usersTable.FindAsync(filter, cancellationToken);

                // Create our resource.
                var user = new User(cacheUser);

                // Update the cache.
                await this.usersCache.Save(user.CacheIds, cacheUser, cancellationToken);

                return user;
            }, cancellationToken);
        }
    }
}