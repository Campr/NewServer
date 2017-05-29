using System;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Connectors.Http;
using Server.Lib.Infrastructure;
using Server.Lib.Models.PostContent;
using Server.Lib.Models.Resources.Api;

namespace Server.Lib.Connectors.Protocol
{
    class ProtocolClient : IProtocolClient
    {
        public ProtocolClient(
            IConstants constants,
            IHttpClient client)
        {
            Ensure.Argument.IsNotNull(constants, nameof(constants));
            Ensure.Argument.IsNotNull(client, nameof(client));

            this.constants = constants;
            this.client = client;
        }

        private readonly IConstants constants;
        private readonly IHttpClient client;
        
        public async Task<ApiPost> FetchPostAtUriAsync(Uri postUri, CancellationToken cancellationToken = new CancellationToken())
        {
            var request = this.client
                .Get(postUri)
                .AddAccept(this.constants.PostContentType);

            var response = await request.PerformAsync<ApiPost>(cancellationToken);
            return await response.ReadContentAsync();
        }

        public async Task<ApiPost<TContent>> FetchPostAtUriAsync<TContent>(Uri postUri, CancellationToken cancellationToken = new CancellationToken()) where TContent : EmptyPostContent
        {
            var request = this.client
                .Get(postUri)
                .AddAccept(this.constants.PostContentType);

            var response = await request.PerformAsync<ApiPost<TContent>>(cancellationToken);
            return await response.ReadContentAsync();
        }
    }
}