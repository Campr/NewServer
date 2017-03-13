using System;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Connectors.Tables;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources.Api;
using Server.Lib.Models.Resources.Cache;
using Server.Lib.ScopeServices;

namespace Server.Lib.Models.Resources
{
    public class User : VersionedResource<CacheUser>
    {
        #region Constructors and private fields.

        public User(IResourceCacheService resourceCacheService)
        {
            Ensure.Argument.IsNotNull(resourceCacheService, nameof(resourceCacheService));
            this.resourceCacheService = resourceCacheService;
        }

        public static User FromCache(
            IResourceCacheService resourceCacheService,
            CacheUser cacheUser)
        {
            return new User(resourceCacheService)
            {
                Id = cacheUser.Id,
                CreatedAt = cacheUser.CreatedAt,
                DeletedAt = cacheUser.DeletedAt,

                VersionId = cacheUser.VersionId,
                OriginalCreatedAt = cacheUser.OriginalCreatedAt,

                Handle = cacheUser.Handle,
                Entity = cacheUser.Entity,
                Email = cacheUser.Email,
                Password = cacheUser.Password,
                PasswordSalt = cacheUser.PasswordSalt,
                IsBotFollowed = cacheUser.IsBotFollowed,
                LastDiscoveryAttempt = cacheUser.LastDiscoveryAttempt
            };
        }

        private readonly IResourceCacheService resourceCacheService;

        #endregion

        public string Handle { get; set; }
        public string Entity { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool? IsBotFollowed { get; set; }
        public DateTime? LastDiscoveryAttempt { get; set; }

        public override string[][] CacheIds => new []
        {
            new [] { "id", this.Id },
            new [] { "id-version", this.Id, this.VersionId },
            new [] { "entity", this.Entity },
            new [] { "email", this.Email }
        };

        public override ApiResource ToApi()
        {
            throw new NotImplementedException();
        }

        public override CacheUser ToCache()
        {
            return new CacheUser
            {
                Id = this.Id,
                CreatedAt = this.CreatedAt,
                DeletedAt = this.DeletedAt,

                VersionId = this.VersionId,
                OriginalCreatedAt = this.OriginalCreatedAt,

                Handle = this.Handle,
                Entity = this.Entity,
                Email = this.Email,
                Password = this.Password,
                PasswordSalt = this.PasswordSalt,
                IsBotFollowed = this.IsBotFollowed,
                LastDiscoveryAttempt = this.LastDiscoveryAttempt
            };
        }

        public override Task SaveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.resourceCacheService.SaveAsync<User, CacheUser>(this, cancellationToken);
        }
    }
}