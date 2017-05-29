using System.Threading;
using System.Threading.Tasks;

namespace Server.Lib.Connectors.Http
{
    public interface IHttpRequest
    {
        IHttpRequest AddAccept(string mediaType);
        Task<IHttpResponse> PerformAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<IHttpResponse<T>> PerformAsync<T>(CancellationToken cancellationToken = default(CancellationToken));
    }
}