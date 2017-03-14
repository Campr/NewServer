using System;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Models.PostContent;
using Server.Lib.Models.Resources.Api;

namespace Server.Lib.Connectors.Protocol
{
    public interface IProtocolClient
    {
        Task<ApiPost> FetchPostAtUriAsync(Uri postUri, CancellationToken cancellationToken = default(CancellationToken));
        Task<ApiPost<TContent>> FetchPostAtUriAsync<TContent>(Uri postUri, CancellationToken cancellationToken = default(CancellationToken)) where TContent : EmptyPostContent;
    }
}