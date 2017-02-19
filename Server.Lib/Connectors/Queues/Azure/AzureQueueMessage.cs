using Microsoft.WindowsAzure.Storage.Queue;
using Server.Lib.Extensions;
using Server.Lib.Infrastructure;

namespace Server.Lib.Connectors.Queues.Azure
{
    public class AzureQueueMessage<T> : IQueueMessage<T> where T : QueueMessageContent
    {
        public AzureQueueMessage(
            CloudQueueMessage baseMessage,
            T content)
        {
            Ensure.Argument.IsNotNull(baseMessage, nameof(baseMessage));
            Ensure.Argument.IsNotNull(content, nameof(content));

            this.BaseMessage = baseMessage;
            this.Content = content;
        }

        public CloudQueueMessage BaseMessage { get; }
        public T Content { get; }
        public int DequeueCount => this.BaseMessage.DequeueCount;
    }
}