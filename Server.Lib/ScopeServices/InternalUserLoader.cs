using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Connectors.Tables;
using Server.Lib.Helpers;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources;
using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.ScopeServices
{
    class InternalUserLoader : IInternalUserLoader
    {
        public InternalUserLoader(
            ITables tables,
            ITextHelpers textHelpers,
            IResourceCacheService resourceCacheService)
        {
            Ensure.Argument.IsNotNull(tables, nameof(tables));
            Ensure.Argument.IsNotNull(textHelpers, nameof(textHelpers));
            Ensure.Argument.IsNotNull(resourceCacheService, nameof(resourceCacheService));
            
            this.textHelpers = textHelpers;
            this.resourceCacheService = resourceCacheService;
            this.userTable = tables.TableForVersionedType<CacheUser>();
        }
        
        private readonly ITextHelpers textHelpers;
        private readonly IResourceCacheService resourceCacheService;
        private readonly IVersionedTable<CacheUser> userTable;

        public User MakeNew(CacheUser cacheUser)
        {
            return User.FromCache(this.resourceCacheService, cacheUser);
        }

        public Task<User> FetchAsync(string userId, CancellationToken cancellationToken)
        {
            Ensure.Argument.IsNotNullOrWhiteSpace(userId, nameof(userId));
            return this.FetchByFilterAsync(this.textHelpers.BuildCacheKey(new [] { "id", userId }), u => u.Id == userId, cancellationToken);
        }

        public Task<User> FetchByEntityAsync(Uri entity, CancellationToken cancellationToken)
        {
            Ensure.Argument.IsNotNull(entity, nameof(entity));
            return this.FetchByFilterAsync(this.textHelpers.BuildCacheKey(new [] { "entity", entity.AbsoluteUri }), u => u.Entity == entity.AbsoluteUri, cancellationToken);
        }

        public Task<User> FetchByHandleAsync(string handle, CancellationToken cancellationToken = new CancellationToken())
        {
            Ensure.Argument.IsNotNullOrWhiteSpace(handle, nameof(handle));
            return this.FetchByFilterAsync(this.textHelpers.BuildCacheKey(new[] { "handle", handle }), u => u.Handle == handle, cancellationToken);
        }

        public Task<User> FetchByEmailAsync(string email, CancellationToken cancellationToken)
        {
            Ensure.Argument.IsNotNullOrWhiteSpace(email, nameof(email));
            return this.FetchByFilterAsync(this.textHelpers.BuildCacheKey(new [] { "email", email }), u => u.Email == email, cancellationToken);
        }

        private Task<User> FetchByFilterAsync(string cacheId, Expression<Func<CacheUser, bool>> filter, CancellationToken cancellationToken)
        {
            return this.resourceCacheService.WrapFetchAsync(cacheId, 
                ct => this.userTable.FindLastVersionAsync(filter, ct),
                (cacheUser, ct) => Task.FromResult(User.FromCache(this.resourceCacheService, cacheUser)),
                cancellationToken);
        }
    }
}