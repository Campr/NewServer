using System.Collections.Generic;

namespace Server.Lib.Models.PostContent.Meta
{
    public class MetaServer
    {
        public string Version { get; set; }
        public long Preference { get; set; }
        public Dictionary<string, string> Urls { get; set; }
    }
}