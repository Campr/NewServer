using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Server.Lib.Extensions;
using Server.Lib.Helpers;
using Server.Lib.Infrastructure;

namespace Server.Lib.Connectors.Queues.Azure
{
    public class AzureQueue<T> : IQueue<T> where T : QueueMessageContent
    {
        public AzureQueue(
            CloudQueue baseQueue,
            IJsonHelpers jsonHelpers)
        {
            Ensure.Argument.IsNotNull(baseQueue, nameof(baseQueue));
            Ensure.Argument.IsNotNull(jsonHelpers, nameof(jsonHelpers));

            this.baseQueue = baseQueue;
            this.jsonHelpers = jsonHelpers;
        }

        private readonly CloudQueue baseQueue;
        private readonly IJsonHelpers jsonHelpers;

        public async Task<IQueueMessage<T>> GetMessageAsync(TimeSpan? visibilityTimeout, CancellationToken cancellationToken)
        {
            // Try to retrieve a message from the underlying queue.
            var message = await this.baseQueue.GetMessageAsync(visibilityTimeout, null, null, cancellationToken);
            if (message == null)
                return null;

            // If a message was found, deserialize it.
            return new AzureQueueMessage<T>(message, this.jsonHelpers.FromJsonString<T>(message.AsString));
        }

        public async Task AddMessageAsync(T content, TimeSpan? initialVisilityDelay, CancellationToken cancellationToken)
        {
            // Serialize the message.
            var stringMessage = this.jsonHelpers.ToJsonString(content);

            // Create the CloudQueueMessage.
            var queueMessage = new CloudQueueMessage(stringMessage);

            // Post it to the queue.
            await this.baseQueue.AddMessageAsync(queueMessage, null, initialVisilityDelay, null, null, cancellationToken);
        }

        public Task AddMessagesAsync(IEnumerable<T> contentList, TimeSpan? initialVisibilityDelay, CancellationToken cancellationToken)
        {
            // Azure does not have batch capabilities. Send in parallel instead.
            var addMessagesTasks = contentList.Select(c => this.AddMessageAsync(c, initialVisibilityDelay, cancellationToken)).ToList();
            return Task.WhenAll(addMessagesTasks);
        }

        public async Task DeleteMessageAsync(IQueueMessage<T> message, CancellationToken cancellationToken)
        {
            // Only do something if this is an Azure message.
            var azureQueueMessage = message as AzureQueueMessage<T>;
            if (azureQueueMessage == null)
                return;

            await this.baseQueue.DeleteMessageAsync(
                azureQueueMessage.BaseMessage.Id,
                azureQueueMessage.BaseMessage.PopReceipt,
                null, null, cancellationToken);
        }
    }
}