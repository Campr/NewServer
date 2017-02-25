using System.Threading;
using System.Threading.Tasks;

namespace Server.Lib.Connectors
{
    public abstract class Connector : IConnector
    {
        public virtual Task InitializeAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(!cancellationToken.IsCancellationRequested);
        }

        public virtual Task ResetAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(!cancellationToken.IsCancellationRequested);
        }
    }
}