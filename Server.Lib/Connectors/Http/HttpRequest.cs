using System;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Helpers;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources.Api;
using Native = System.Net.Http;

namespace Server.Lib.Connectors.Http
{
    class HttpRequest : IHttpRequest
    {
        public HttpRequest(
            IHttpHelpers httpHelpers,
            Native.HttpClient client, 
            Native.HttpMethod method, 
            Uri target)
        {
            Ensure.Argument.IsNotNull(httpHelpers, nameof(httpHelpers));
            Ensure.Argument.IsNotNull(client, nameof(client));
            Ensure.Argument.IsNotNull(target, nameof(target));

            this.httpHelpers = httpHelpers;
            this.client = client;
            this.request = new Native.HttpRequestMessage(method, target);
        }

        private readonly IHttpHelpers httpHelpers;
        private readonly Native.HttpClient client;
        private readonly Native.HttpRequestMessage request;

        public async Task<IHttpResponse> PerformAsync(CancellationToken cancellationToken)
        {
            var response = await this.client.SendAsync(this.request, cancellationToken);
            return new HttpResponse(this.httpHelpers, response);
        }
    }

    class HttpRequest<T> : HttpRequest, IHttpRequest<T> where T : ApiResource
    {
        public HttpRequest(
            IJsonHelpers jsonHelpers,
            IHttpHelpers httpHelpers,
            Native.HttpClient client, 
            Native.HttpMethod method, 
            Uri target) : base(httpHelpers, client, method, target)
        {
            Ensure.Argument.IsNotNull(jsonHelpers, nameof(jsonHelpers));
            this.jsonHelpers = jsonHelpers;
        }

        private readonly IJsonHelpers jsonHelpers;

        public new async Task<T> PerformAsync(CancellationToken cancellationToken)
        {
            var response = await base.PerformAsync(cancellationToken);

            // If the request was successful, parse the result.
            if (response.IsSuccessStatusCode)
            {
                var content = await response.ReadContentAsStringAsync();
                return this.jsonHelpers.FromJsonString<T>(content);
            }

            // Otherwise, it may be a permanently unavailable resource.
            if (!response.IsRetryableStatusCode)
                return null;

            // If it's a temporary failure, throw.
            throw new HttpException(response.StatusCode, "Temporary HTTP failure.");
        }
    }
}