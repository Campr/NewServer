using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources.Cache.Posts;
using Server.Lib.ScopeServices;

namespace Server.Lib.Models.Resources.Posts
{
    public class PostReference
    {
        #region Constructors

        public static async Task<PostReference> FromCacheAsync(
            IUserLoader userLoader, 
            CachePostReference cachePostReference,
            CancellationToken cancellationToken)
        {
            Ensure.Argument.IsNotNull(userLoader, nameof(userLoader));
            Ensure.Argument.IsNotNull(cachePostReference, nameof(cachePostReference));

            // Retrieve dependencies.
            var user = await userLoader.FetchAsync(cachePostReference.UserId, cancellationToken);
            Ensure.Dependency.IsNotNull(user, nameof(user));

            return new PostReference
            {
                User = user,
                OriginalEntity = cachePostReference.OriginalEntity,

                PostId = cachePostReference.PostId,
                VersionId = cachePostReference.VersionId,
                Type = PostType.FromString(cachePostReference.Type),

                Role = cachePostReference.Role,
                IsPublic = cachePostReference.IsPublic
            };
        }

        #endregion

        #region Public properties.

        // User.
        public User User { get; private set; }
        public string OriginalEntity { get; private set; }

        // Post.
        public string PostId { get; private set; }
        public string VersionId { get; private set; }
        public PostType Type { get; private set; }

        // Reference.
        public string Role { get; private set; }
        public bool IsPublic { get; private set; }

        #endregion

        #region Serialization methods.

        public CachePostReference ToCache()
        {
            return new CachePostReference
            {
                UserId = this.User.Id,
                OriginalEntity = this.OriginalEntity,

                PostId = this.PostId,
                VersionId = this.VersionId,
                Type = this.Type.ToString(),

                Role = this.Role,
                IsPublic = this.IsPublic
            };
        }

        #endregion
    }
}