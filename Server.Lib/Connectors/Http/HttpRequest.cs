using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Helpers;
using Server.Lib.Infrastructure;
using Native = System.Net.Http;

namespace Server.Lib.Connectors.Http
{
    class HttpRequest : IHttpRequest
    {
        public HttpRequest(
            IJsonHelpers jsonHelpers,
            IHttpHelpers httpHelpers,
            Native.HttpClient client, 
            Native.HttpMethod method, 
            Uri target)
        {
            Ensure.Argument.IsNotNull(jsonHelpers, nameof(jsonHelpers));
            Ensure.Argument.IsNotNull(httpHelpers, nameof(httpHelpers));
            Ensure.Argument.IsNotNull(client, nameof(client));
            Ensure.Argument.IsNotNull(target, nameof(target));

            this.jsonHelpers = jsonHelpers;
            this.httpHelpers = httpHelpers;
            this.client = client;
            this.request = new Native.HttpRequestMessage(method, target);
        }

        private readonly IJsonHelpers jsonHelpers;
        private readonly IHttpHelpers httpHelpers;
        private readonly Native.HttpClient client;
        private readonly Native.HttpRequestMessage request;

        public IHttpRequest AddAccept(string mediaType)
        {
            Ensure.Argument.IsNotNullOrWhiteSpace(mediaType, nameof(mediaType));

            // Add a new accept header to the underlying request.
            this.request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            return this;
        }

        public async Task<IHttpResponse> PerformAsync(CancellationToken cancellationToken)
        {
            var response = await this.client.SendAsync(this.request, cancellationToken);
            return new HttpResponse(this.httpHelpers, response);
        }

        public async Task<IHttpResponse<T>> PerformAsync<T>(CancellationToken cancellationToken)
        {
            var response = await this.client.SendAsync(this.request, cancellationToken);
            return new HttpResponse<T>(this.jsonHelpers, this.httpHelpers, response);
        }
    }
}