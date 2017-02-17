using System.Threading;
using System.Threading.Tasks;

namespace Server.Lib.Connectors
{
    public interface IConnector
    {
        Task InitializeAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}