using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources.Cache.Posts;
using Server.Lib.ScopeServices;

namespace Server.Lib.Models.Resources.Posts
{
    public class PostAttachment
    {
        private PostAttachment()
        {
        }

        public static async Task<PostAttachment> FromCacheAsync(
            IAttachmentLoader attachmentLoader,
            CachePostAttachment cachePostAttachment,
            CancellationToken cancellationToken)
        {
            Ensure.Argument.IsNotNull(attachmentLoader, nameof(attachmentLoader));
            Ensure.Argument.IsNotNull(cachePostAttachment, nameof(cachePostAttachment));

            // Load dependencies.
            var attachment = await attachmentLoader.FetchAsync(cachePostAttachment.AttachmentId, cancellationToken);
            Ensure.Dependency.IsNotNull(attachment, nameof(attachment));

            // Create the new post attachment.
            return new PostAttachment
            {
                Attachment = attachment,
                Category = cachePostAttachment.Category,
                Name = cachePostAttachment.Name,
                ContentType = cachePostAttachment.ContentType
            };
        }

        public Attachment Attachment { get; private set; }
        public string Category { get; private set; }
        public string Name { get; private set; }
        public string ContentType { get; private set; }
    }
}