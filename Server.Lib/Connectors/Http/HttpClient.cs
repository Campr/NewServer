using System;
using Server.Lib.Helpers;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources.Api;
using Native = System.Net.Http;

namespace Server.Lib.Connectors.Http
{
    class HttpClient : IHttpClient
    {
        public HttpClient(
            IJsonHelpers jsonHelpers, 
            IHttpHelpers httpHelpers)
        {
            Ensure.Argument.IsNotNull(jsonHelpers, nameof(jsonHelpers));
            Ensure.Argument.IsNotNull(httpHelpers, nameof(httpHelpers));

            this.jsonHelpers = jsonHelpers;
            this.httpHelpers = httpHelpers;

            // Create the underlying HttpClient.
            this.client = new Native.HttpClient();
        }

        private readonly IJsonHelpers jsonHelpers;
        private readonly IHttpHelpers httpHelpers;
        private readonly Native.HttpClient client;

        public IHttpRequest Head(Uri target)
        {
            return new HttpRequest(this.httpHelpers, this.client, Native.HttpMethod.Head, target);
        }

        public IHttpRequest Get(Uri target)
        {
            return new HttpRequest(this.httpHelpers, this.client, Native.HttpMethod.Get, target);
        }

        public IHttpRequest<T> Get<T>(Uri target) where T : ApiResource
        {
            return new HttpRequest<T>(this.jsonHelpers, this.httpHelpers, this.client, Native.HttpMethod.Get, target);
        }
    }
}