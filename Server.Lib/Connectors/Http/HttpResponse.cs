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

    class HttpResponse<T> : HttpResponse, IHttpResponse<T>
    {
        public HttpResponse(
            IJsonHelpers jsonHelpers,
            IHttpHelpers httpHelpers, 
            Native.HttpResponseMessage response) 
                : base(httpHelpers, response)
        {
            Ensure.Argument.IsNotNull(jsonHelpers, nameof(jsonHelpers));
            this.jsonHelpers = jsonHelpers;
        }

        private readonly IJsonHelpers jsonHelpers;

        public async Task<T> ReadContentAsync()
        {
            // If the request was successful, parse the result.
            if (this.IsSuccessStatusCode)
            {
                var content = await this.ReadContentAsStringAsync();
                return this.jsonHelpers.FromJsonString<T>(content);
            }

            // Otherwise, it may be a permanently unavailable resource.
            if (!this.IsRetryableStatusCode)
                return default(T);

            // If it's a temporary failure, throw.
            throw new HttpException(this.StatusCode, "Temporary HTTP failure.");
        }
    }
}