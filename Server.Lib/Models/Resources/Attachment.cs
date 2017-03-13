using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources.Api;
using Server.Lib.Models.Resources.Cache;
using Server.Lib.ScopeServices;

namespace Server.Lib.Models.Resources
{
    public class Attachment : Resource<CacheAttachment>
    {
        public Attachment(IResourceCacheService resourceCacheService)
        {
            Ensure.Argument.IsNotNull(resourceCacheService, nameof(resourceCacheService));
            this.resourceCacheService = resourceCacheService;
        }

        public static Attachment FromCache(
            IResourceCacheService resourceCacheService,
            CacheAttachment cacheAttachment)
        {
            Ensure.Argument.IsNotNull(cacheAttachment, nameof(cacheAttachment));

            return new Attachment(resourceCacheService)
            {
                Id = cacheAttachment.Id,
                CreatedAt = cacheAttachment.CreatedAt,
                Size = cacheAttachment.Size
            };
        }

        private readonly IResourceCacheService resourceCacheService;

        public long Size { get; set; }

        public override ApiResource ToApi()
        {
            throw new System.NotImplementedException();
        }

        public override CacheAttachment ToCache()
        {
            return new CacheAttachment
            {
                Id = this.Id,
                CreatedAt = this.CreatedAt,
                Size = this.Size
            };
        }

        public override Task SaveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.resourceCacheService.SaveAsync<Attachment, CacheAttachment>(this, cancellationToken);
        }
    }
}