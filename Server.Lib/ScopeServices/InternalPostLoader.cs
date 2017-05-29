using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Connectors.Tables;
using Server.Lib.Helpers;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources;
using Server.Lib.Models.Resources.Cache;
using Server.Lib.Services;

namespace Server.Lib.ScopeServices
{
    class InternalPostLoader : IInternalPostLoader
    {
        public InternalPostLoader(
            ITables tables,
            IPostLicenseLoader postLicenseLoader,
            ITextHelpers textHelpers,
            IResourceCacheService resourceCacheService,
            IInternalUserLoader internalUserLoader,
            IAttachmentLoader attachmentLoader)
        {
            Ensure.Argument.IsNotNull(tables, nameof(tables));
            Ensure.Argument.IsNotNull(postLicenseLoader, nameof(postLicenseLoader));
            Ensure.Argument.IsNotNull(textHelpers, nameof(textHelpers));
            Ensure.Argument.IsNotNull(resourceCacheService, nameof(resourceCacheService));
            Ensure.Argument.IsNotNull(internalUserLoader, nameof(internalUserLoader));
            Ensure.Argument.IsNotNull(attachmentLoader, nameof(attachmentLoader));

            this.textHelpers = textHelpers;
            this.resourceCacheService = resourceCacheService;
            this.internalUserLoader = internalUserLoader;
            this.attachmentLoader = attachmentLoader;
            this.postLicenseLoader = postLicenseLoader;
            this.postTable = tables.TableForVersionedType<CachePost>();
        }

        private readonly ITextHelpers textHelpers;
        private readonly IResourceCacheService resourceCacheService;
        private readonly IInternalUserLoader internalUserLoader;
        private readonly IAttachmentLoader attachmentLoader;
        private readonly IPostLicenseLoader postLicenseLoader;
        private readonly IVersionedTable<CachePost> postTable;

        public Task<Post> FetchAsync(User user, string postId, CancellationToken cancellationToken)
        {
            Ensure.Argument.IsNotNull(user, nameof(user));
            Ensure.Argument.IsNotNullOrWhiteSpace(postId, nameof(postId));

            // Build the cacheId from the provided variables.
            var cacheId = this.textHelpers.BuildCacheKey(new[] { "user-id", user.Id, postId });

            // Fetch the corresponding post.
            return this.resourceCacheService.WrapFetchAsync(cacheId, 
                ct => this.postTable.FindLastVersionAsync(p => p.UserId == user.Id && p.Id == postId, ct),
                this.FromCacheAsync, 
                cancellationToken);
        }

        public Task<Post> FetchAsync(User user, string postId, string versionId, CancellationToken cancellationToken)
        {
            Ensure.Argument.IsNotNull(user, nameof(user));
            Ensure.Argument.IsNotNullOrWhiteSpace(postId, nameof(postId));
            Ensure.Argument.IsNotNullOrWhiteSpace(versionId, nameof(versionId));

            // Build the cacheId from the provided variables.
            var cacheId = this.textHelpers.BuildCacheKey(new [] { "user-id-version", user.Id, postId, versionId });

            // Fetch the corresponding post.
            return this.resourceCacheService.WrapFetchAsync(cacheId, 
                ct => this.postTable.FindAsync(p => p.UserId == user.Id && p.Id == postId && p.VersionId == versionId, ct),
                this.FromCacheAsync,
                cancellationToken);
        }

        private Task<Post> FromCacheAsync(CachePost cachePost, CancellationToken cancellationToken)
        {
            return Post.FromCacheAsync(
                this.resourceCacheService,
                this.internalUserLoader, 
                this.attachmentLoader,
                this.postLicenseLoader, 
                cachePost, 
                cancellationToken);
        }
    }
}