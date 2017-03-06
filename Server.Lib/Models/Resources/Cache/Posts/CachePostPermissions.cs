using System.Collections.Generic;

namespace Server.Lib.Models.Resources.Cache.Posts
{
    public class CachePostPermissions
    {
        public bool IsPublic { get; set; }
        public List<string> UserIds { get; set; }
        public List<CachePostReference> Groups { get; set; }
    }
}