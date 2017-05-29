using System;
using System.Collections.Generic;
using Server.Lib.Models.Resources.Cache.Posts;

namespace Server.Lib.Models.Resources.Cache
{
    public class CachePost : VersionedCacheResource
    {
        // User.
        public string UserId { get; set; }
        public string OriginalEntity { get; set; }

        // Version.
        public string Type { get; set; }
        public CachePostPermissions Permissions { get; set; }
        public List<CachePostReference> Links { get; set; }
        public List<CachePostAttachment> Attachments { get; set; }
        public List<string> Licenses { get; set; }

        // Dates.
        public DateTime PublishedAt { get; set; }
        public DateTime OriginalPublishedAt { get; set; }

        // Content.
        public List<string> ContentUrls { get; set; }
    }
}