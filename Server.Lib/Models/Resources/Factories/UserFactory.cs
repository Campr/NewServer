using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Connectors.Db;
using Server.Lib.Helpers;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources.Cache;
using Server.Lib.ScopeServices;

namespace Server.Lib.Models.Resources.Factories
{
    class UserFactory : IUserFactory
    {
        public UserFactory(
            ITables tables,
            ITextHelpers textHelpers,
            IResourceCacheService resourceCacheService)
        {
            Ensure.Argument.IsNotNull(tables, nameof(tables));
            Ensure.Argument.IsNotNull(textHelpers, nameof(textHelpers));
            Ensure.Argument.IsNotNull(resourceCacheService, nameof(resourceCacheService));
            
            this.usersTable = tables.Users;
            this.textHelpers = textHelpers;
            this.resourceCacheService = resourceCacheService;
        }
        
        private readonly IVersionedTable<CacheUser> usersTable;
        private readonly ITextHelpers textHelpers;
        private readonly IResourceCacheService resourceCacheService;

        public User MakeNew()
        {
            throw new System.NotImplementedException();
        }

        public Task<User> FetchAsync(string userId, CancellationToken cancellationToken)
        {
            return this.FetchByFilterAsync(this.textHelpers.BuildCacheKey(new [] { "id", userId }), u => u.Id == userId, cancellationToken);
        }

        public Task<User> FetchByEntityAsync(string entity, CancellationToken cancellationToken)
        {
            return this.FetchByFilterAsync(this.textHelpers.BuildCacheKey(new[] { "entity", entity }), u => u.Entity == entity, cancellationToken);
        }

        public Task<User> FetchByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return this.FetchByFilterAsync(this.textHelpers.BuildCacheKey(new[] { "email", email }), u => u.Email == email, cancellationToken);
        }

        private Task<User> FetchByFilterAsync(string cacheId, Expression<Func<CacheUser, bool>> filter, CancellationToken cancellationToken)
        {
            return this.resourceCacheService.WrapFetchAsync(cacheId, ct =>
                    this.usersTable.FindLastVersionAsync(filter, ct)
                , (cacheUser, ct) =>
                    Task.FromResult(new User(cacheUser))
                , cancellationToken);
        }
    }
}