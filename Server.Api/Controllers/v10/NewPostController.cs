using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Server.Lib.Models.Resources.Api;

namespace Server.Api.Controllers.v10
{
    [Route("/v1.0/{userHandle}/posts")]
    public class NewPostController : Controller
    {
        // POST /posts
        [HttpPost]
        public async Task<HttpResponseMessage> NewPost([FromBody]ApiPost apiPost)
        {
            return null;
        }
    }
}