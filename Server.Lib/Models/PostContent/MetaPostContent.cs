using System;
using System.Collections.Generic;
using Server.Lib.Models.PostContent.Meta;

namespace Server.Lib.Models.PostContent
{
    public class MetaPostContent : EmptyPostContent
    {
        public Uri Entity { get; set; }
        public List<Uri> PreviousEntities { get; set; }
        public MetaProfile Profile { get; set; }
        public List<MetaServer> Servers { get; set; }
    }
}