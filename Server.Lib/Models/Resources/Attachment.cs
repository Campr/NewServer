using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources.Api;
using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Models.Resources
{
    public class Attachment : Resource<CacheAttachment>
    {
        public static Attachment FromCache(CacheAttachment cacheAttachment)
        {
            Ensure.Argument.IsNotNull(cacheAttachment, nameof(cacheAttachment));

            return new Attachment
            {
                Id = cacheAttachment.Id,
                CreatedAt = cacheAttachment.CreatedAt,
                Size = cacheAttachment.Size
            };
        }

        public long Size { get; set; }

        public override ApiResource ToApi()
        {
            throw new System.NotImplementedException();
        }

        public override CacheAttachment ToDb()
        {
            throw new System.NotImplementedException();
        }
    }
}