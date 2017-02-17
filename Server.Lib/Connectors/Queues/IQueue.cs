using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Lib.Connectors.Queues
{
    public interface IQueue<T> where T : QueueMessageContent
    {
        Task<IQueueMessage<T>> GetMessageAsync(TimeSpan? visibilityTimeout = null, CancellationToken cancellationToken = default(CancellationToken));
        Task AddMessageAsync(T content, TimeSpan? initialVisilityDelay = null, CancellationToken cancellationToken = default(CancellationToken));
        Task AddMessagesAsync(IEnumerable<T> contentList, TimeSpan? initialVisibilityDelay = null, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteMessageAsync(IQueueMessage<T> message, CancellationToken cancellationToken = default(CancellationToken));
    }
}