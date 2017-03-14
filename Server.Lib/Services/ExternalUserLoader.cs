using System;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Connectors.Http;
using Server.Lib.Connectors.Protocol;
using Server.Lib.Helpers;
using Server.Lib.Infrastructure;
using Server.Lib.Models.PostContent;
using Server.Lib.Models.Resources;
using Server.Lib.ScopeServices;

namespace Server.Lib.Services
{
    class ExternalUserLoader : IExternalUserLoader
    {
        public ExternalUserLoader(
            IConstants constants,
            ITextHelpers textHelpers,
            IHttp http,
            IProtocolClient protocolClient,
            IResourceCacheService resourceCacheService)
        {
            Ensure.Argument.IsNotNull(constants, nameof(constants));
            Ensure.Argument.IsNotNull(textHelpers, nameof(textHelpers));
            Ensure.Argument.IsNotNull(http, nameof(http));
            Ensure.Argument.IsNotNull(protocolClient, nameof(protocolClient));
            Ensure.Argument.IsNotNull(resourceCacheService, nameof(resourceCacheService));

            this.constants = constants;
            this.textHelpers = textHelpers;
            this.http = http;
            this.protocolClient = protocolClient;
            this.resourceCacheService = resourceCacheService;
        }

        private readonly IConstants constants;
        private readonly ITextHelpers textHelpers;
        private readonly IHttp http;
        private readonly IProtocolClient protocolClient;
        private readonly IResourceCacheService resourceCacheService;
        
        public async Task<User> FetchByEntity(Uri entity, CancellationToken cancellationToken)
        {
            Ensure.Argument.IsNotNull(entity, nameof(entity));

            // Perform discovery on the specified entity.
            var client = this.http.MakeClient();
            var request = client.Head(entity);
            var response = await request.PerformAsync(cancellationToken);

            // Extract the Uri of the Meta post from the response.
            var metaPostUri = response.FindLinkInHeader(this.constants.MetaPostRel);

            // If needed, combine this Uri to get the absolute Meta post Uri.
            var absoluteMetaPostUri = metaPostUri.IsAbsoluteUri
                ? metaPostUri
                : new Uri(entity, metaPostUri);

            // Use the protocol client to fetch the Meta post.
            var metaApiPost = await this.protocolClient.FetchPostAtUriAsync<MetaPostContent>(absoluteMetaPostUri, cancellationToken);
            if (metaApiPost == null)
                return null;

            // Create a new User from the MetaPost we found.
            return User.FromMetaPost(this.textHelpers, this.resourceCacheService, metaApiPost);
        }
    }
}