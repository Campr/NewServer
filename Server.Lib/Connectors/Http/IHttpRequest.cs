using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Models.Resources.Api;

namespace Server.Lib.Connectors.Http
{
    public interface IHttpRequest
    {
        Task<IHttpResponse> PerformAsync(CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IHttpRequest<T> : IHttpRequest where T : ApiResource
    {
        new Task<T> PerformAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}