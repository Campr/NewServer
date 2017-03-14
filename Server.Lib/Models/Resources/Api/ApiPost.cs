using Newtonsoft.Json.Linq;
using Server.Lib.Models.PostContent;

namespace Server.Lib.Models.Resources.Api
{
    public class ApiPost : ApiResource
    {
        public string Id { get; set; }
        public string Entity { get; set; }
        public string OriginalEntity { get; set; }
        public JObject Content { get; set; }
    }

    public class ApiPost<TContent> : ApiPost where TContent : EmptyPostContent
    {
        public new TContent Content { get; set; }   
    }
}