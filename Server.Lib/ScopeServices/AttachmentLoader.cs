using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Connectors.Tables;
using Server.Lib.Helpers;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources;
using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.ScopeServices
{
    class AttachmentLoader : IAttachmentLoader
    {
        public AttachmentLoader(
            ITables tables,
            ITextHelpers textHelpers,
            IResourceCacheService resourceCacheService)
        {
            Ensure.Argument.IsNotNull(tables, nameof(tables));
            Ensure.Argument.IsNotNull(textHelpers, nameof(textHelpers));
            Ensure.Argument.IsNotNull(resourceCacheService, nameof(resourceCacheService));

            this.textHelpers = textHelpers;
            this.resourceCacheService = resourceCacheService;
            this.attachmentsTable = tables.TableForType<CacheAttachment>();
        }

        private readonly ITextHelpers textHelpers;
        private readonly IResourceCacheService resourceCacheService;
        private readonly ITable<CacheAttachment> attachmentsTable;
        
        public Task<Attachment> FetchAsync(string id, CancellationToken cancellationToken)
        {
            Ensure.Argument.IsNotNullOrWhiteSpace(id, nameof(id));

            // Build the cacheId for this attachment.
            var cacheId = this.textHelpers.BuildCacheKey(new [] { "id", id });
            
            // Fetch the resource.
            return this.resourceCacheService.WrapFetchAsync(cacheId,
                ct => this.attachmentsTable.FindAsync(a => a.Id == id, ct),
                (cacheAttachment, ct) => Task.FromResult(Attachment.FromCache(cacheAttachment)),
                cancellationToken);
        }
    }
}