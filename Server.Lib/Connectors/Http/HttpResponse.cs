using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Server.Lib.Helpers;
using Server.Lib.Infrastructure;
using Native = System.Net.Http;

namespace Server.Lib.Connectors.Http
{
    class HttpResponse : IHttpResponse
    {
        public HttpResponse(
            IHttpHelpers httpHelpers,
            Native.HttpResponseMessage response)
        {
            Ensure.Argument.IsNotNull(httpHelpers, nameof(httpHelpers));
            Ensure.Argument.IsNotNull(response, nameof(response));

            this.httpHelpers = httpHelpers;
            this.response = response;
        }

        private readonly IHttpHelpers httpHelpers;
        private readonly Native.HttpResponseMessage response;

        public HttpStatusCode StatusCode => this.response.StatusCode;
        public bool IsSuccessStatusCode => this.response.IsSuccessStatusCode;
        public bool IsRetryableStatusCode => this.response.StatusCode == HttpStatusCode.InternalServerError
            || this.response.StatusCode == HttpStatusCode.BadGateway
            || this.response.StatusCode == HttpStatusCode.ServiceUnavailable
            || this.response.StatusCode == HttpStatusCode.GatewayTimeout;

        public Task<string> ReadContentAsStringAsync()
        {
            return this.response.Content.ReadAsStringAsync();
        }

        public Uri FindLinkInHeader(string linkRel)
        {
            var links = this.httpHelpers.ReadLinksInHeaders(this.response.Headers, linkRel);
            return links.FirstOrDefault();
        }
    }
}