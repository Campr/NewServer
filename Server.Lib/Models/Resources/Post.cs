using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources.Api;
using Server.Lib.Models.Resources.Cache;
using Server.Lib.Models.Resources.Posts;
using Server.Lib.ScopeServices;
using Server.Lib.Services;

namespace Server.Lib.Models.Resources
{
    public class Post : VersionedResource<CachePost>
    {
        #region Constructors and private fields.

        public Post(IResourceCacheService resourceCacheService)
        {
            Ensure.Argument.IsNotNull(resourceCacheService, nameof(resourceCacheService));
            this.resourceCacheService = resourceCacheService;
        }

        public static async Task<Post> FromCacheAsync(
            IResourceCacheService resourceCacheService,
            IInternalUserLoader internalUserLoader, 
            IAttachmentLoader attachmentLoader,
            IPostLicenseLoader postLicenseLoader,
            CachePost cachePost,
            CancellationToken cancellationToken)
        {
            Ensure.Argument.IsNotNull(internalUserLoader, nameof(internalUserLoader));
            Ensure.Argument.IsNotNull(attachmentLoader, nameof(attachmentLoader));
            Ensure.Argument.IsNotNull(postLicenseLoader, nameof(postLicenseLoader));
            Ensure.Argument.IsNotNull(cachePost, nameof(cachePost));

            // Resolve dependencies.
            var userTask = internalUserLoader.FetchAsync(cachePost.UserId, cancellationToken);
            var permissionsTask = PostPermissions.FromCacheAsync(internalUserLoader, cachePost.Permissions, cancellationToken);
            var linksTasks = cachePost.Links?.Select(r => PostReference.FromCacheAsync(internalUserLoader, r, cancellationToken)).ToList() ?? new List<Task<PostReference>>();
            var attachmentsTasks = cachePost.Attachments?.Select(a => PostAttachment.FromCacheAsync(attachmentLoader, a, cancellationToken)).ToList() ?? new List<Task<PostAttachment>>();
            var licensesTasks = cachePost.Licenses?.Select(l => postLicenseLoader.LoadAsync(l, cancellationToken)).ToList() ?? new List<Task<PostLicense>>();

            await Task.WhenAll(
                userTask,
                permissionsTask,
                Task.WhenAll(linksTasks),
                Task.WhenAll(attachmentsTasks),
                Task.WhenAll(licensesTasks)
            );

            // Make sure all of our required dependencies are here.
            var user = userTask.Result;
            Ensure.Dependency.IsNotNull(user, nameof(user));

            var licenses = licensesTasks.Select(t => t.Result).ToList();
            Ensure.Dependency.IsNotNull(licenses, nameof(licenses));

            return new Post(resourceCacheService)
            {
                Id = cachePost.Id,
                VersionId = cachePost.VersionId,

                CreatedAt = cachePost.CreatedAt,
                DeletedAt = cachePost.DeletedAt,
                OriginalCreatedAt = cachePost.OriginalCreatedAt,

                // User.
                User = user,
                OriginalEntity = cachePost.OriginalEntity,

                // Version.
                Type = PostType.FromString(cachePost.Type),
                Permissions = permissionsTask.Result,
                Links = linksTasks.Select(t => t.Result).ToList(),
                Attachments = attachmentsTasks.Select(t => t.Result).ToList(),
                Licenses = licenses,

                // Dates.
                PublishedAt = cachePost.PublishedAt,
                OriginalPublishedAt = cachePost.OriginalPublishedAt
            };
        }

        private readonly IResourceCacheService resourceCacheService;

        #endregion

        #region Public properties.

        // User.
        public User User { get; private set; }
        public string OriginalEntity { get; private set; }

        // Version.
        public PostType Type { get; private set; }
        public List<PostReference> Links { get; private set; }
        public List<PostAttachment> Attachments { get; private set; }
        public PostPermissions Permissions { get; private set; }
        public List<PostLicense> Licenses { get; private set; }

        // Dates.
        public DateTime PublishedAt { get; private set; }
        public DateTime OriginalPublishedAt { get; private set; }

        //// Content.
        //public T LoadContent<T>()
        //{
            
        //}

        #endregion

        protected override string[][] AllCacheIds => new []
        {
            new [] { "user-id", this.User.Id, this.Id },
            new [] { "user-id-version", this.User.Id, this.Id, this.VersionId }
        };

        public override ApiResource ToApi()
        {
            throw new System.NotImplementedException();
        }

        public override CachePost ToCache()
        {
            return new CachePost
            {
                Id = this.Id,
                VersionId = this.VersionId,

                CreatedAt = this.CreatedAt,
                DeletedAt = this.DeletedAt,
                OriginalCreatedAt = this.OriginalPublishedAt,

                // User.
                UserId = this.User.Id,
                OriginalEntity = this.OriginalEntity,

                // Version.
                Type = this.Type.ToString(),
                Permissions = this.Permissions.ToCache(),
                Links = this.Links.Select(l => l.ToCache()).ToList(),
                Attachments = this.Attachments.Select(a => a.ToCache()).ToList(),
                Licenses = this.Licenses.Select(l => l.ToCache()).ToList(),

                // Dates.
                PublishedAt = this.PublishedAt,
                OriginalPublishedAt = this.OriginalPublishedAt
            };
        }

        public override Task SaveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.resourceCacheService.SaveAsync<Post, CachePost>(this, cancellationToken);
        }
    }
}