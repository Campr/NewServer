using System.Threading;
using System.Threading.Tasks;

namespace Server.Lib.Connectors
{
    public abstract class Connector : IConnector
    {
        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(!cancellationToken.IsCancellationRequested);
        }
    }
}