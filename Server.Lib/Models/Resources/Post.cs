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
        #region Constructors.

        public static async Task<Post> FromCacheAsync(
            IUserLoader userLoader, 
            IAttachmentLoader attachmentLoader,
            IPostLicenseLoader postLicenseLoader,
            CachePost cachePost,
            CancellationToken cancellationToken)
        {
            Ensure.Argument.IsNotNull(userLoader, nameof(userLoader));
            Ensure.Argument.IsNotNull(attachmentLoader, nameof(attachmentLoader));
            Ensure.Argument.IsNotNull(postLicenseLoader, nameof(postLicenseLoader));
            Ensure.Argument.IsNotNull(cachePost, nameof(cachePost));

            // Resolve dependencies.
            var userTask = userLoader.FetchAsync(cachePost.UserId, cancellationToken);
            var permissionsTask = PostPermissions.FromCacheAsync(userLoader, cachePost.Permissions, cancellationToken);
            var linksTasks = cachePost.Links?.Select(r => PostReference.FromCacheAsync(userLoader, r, cancellationToken)).ToList() ?? new List<Task<PostReference>>();
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

            return new Post
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


        public override string[][] CacheIds => new []
        {
            new [] { "user-id", this.User.Id, this.Id },
            new [] { "user-id-version", this.User.Id, this.Id, this.VersionId }
        };

        public override ApiResource ToApi()
        {
            throw new System.NotImplementedException();
        }

        public override CachePost ToDb()
        {
            throw new System.NotImplementedException();
        }
    }
}