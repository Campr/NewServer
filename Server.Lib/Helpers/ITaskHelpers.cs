using System;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Lib.Helpers
{
    public interface ITaskHelpers
    {
        Task RetryAsync(Func<Task> worker, CancellationToken cancellationToken = default(CancellationToken));
        Task<TResult> RetryAsync<TResult>(Func<Task<TResult>> worker, CancellationToken cancellationToken = default(CancellationToken));
    }
}