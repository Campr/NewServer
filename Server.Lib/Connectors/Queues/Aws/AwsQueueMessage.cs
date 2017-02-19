using Amazon.SQS.Model;
using Server.Lib.Extensions;
using Server.Lib.Infrastructure;

namespace Server.Lib.Connectors.Queues.Aws
{
    public class AwsQueueMessage<T> : IQueueMessage<T> where T : QueueMessageContent
    {
        public AwsQueueMessage(
            Message baseMessage,
            T content)
        {
            Ensure.Argument.IsNotNull(baseMessage, nameof(baseMessage));
            Ensure.Argument.IsNotNull(content, nameof(content));

            this.BaseMessage = baseMessage;
            this.Content = content;

            this.DequeueCount = baseMessage.Attributes
                .TryGetValue("ApproximateReceiveCount")
                .TryParseInt()
                .GetValueOrDefault();
        }

        public Message BaseMessage { get; }
        public T Content { get; }
        public int DequeueCount { get; }
    }
}