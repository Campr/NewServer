using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Server.Lib.Extensions;
using Server.Lib.Helpers;

namespace Server.Lib.Connectors.Queues.Aws
{
    public class AwsQueue<T> : IQueue<T> where T : QueueMessageContent
    {
        public AwsQueue(IJsonHelpers jsonHelpers,
            ITextHelpers textHelpers,
            AmazonSQSClient client,
            string queueUrl)
        {
            Ensure.Argument.IsNotNull(jsonHelpers, nameof(jsonHelpers));
            Ensure.Argument.IsNotNull(textHelpers, nameof(textHelpers));
            Ensure.Argument.IsNotNull(client, nameof(client));
            Ensure.Argument.IsNotNullOrWhiteSpace(queueUrl, nameof(queueUrl));

            this.jsonHelpers = jsonHelpers;
            this.textHelpers = textHelpers;
            this.client = client;
            this.queueUrl = queueUrl;
        }

        private readonly IJsonHelpers jsonHelpers;
        private readonly ITextHelpers textHelpers;
        private readonly AmazonSQSClient client;
        private readonly string queueUrl;

        public async Task<IQueueMessage<T>> GetMessageAsync(TimeSpan? visibilityTimeout, CancellationToken cancellationToken)
        {
            // Perform the request.
            var response = await this.client.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = this.queueUrl,
                WaitTimeSeconds = 4,
                AttributeNames = { "ApproximateReceiveCount" },
                VisibilityTimeout = (int)visibilityTimeout.GetValueOrDefault(TimeSpan.FromSeconds(30)).TotalSeconds
            }, cancellationToken);

            // If no messages were returned, no need to continue.
            var firstMessage = response.Messages.FirstOrDefault();
            if (firstMessage == null)
                return null;

            // Create the wrapper and return.
            return new AwsQueueMessage<T>(firstMessage, this.jsonHelpers.FromJsonString<T>(firstMessage.Body));
        }

        public Task AddMessageAsync(T content, TimeSpan? initialVisilityDelay, CancellationToken cancellationToken)
        {
            Ensure.Argument.IsNotNull(content, nameof(content));

            // Post to the queue.
            return this.client.SendMessageAsync(new SendMessageRequest
            {
                QueueUrl = this.queueUrl,
                DelaySeconds = (int)initialVisilityDelay.GetValueOrDefault().TotalSeconds,
                MessageBody = this.jsonHelpers.ToJsonString(content)
            }, cancellationToken);
        }

        public Task AddMessagesAsync(IEnumerable<T> contentList, TimeSpan? initialVisibilityDelay, CancellationToken cancellationToken)
        {
            Ensure.Argument.IsNotNull(contentList, nameof(contentList));

            // Send this in batches of 10 messages.
            var sendMessagesTasks = contentList.Batch(10).Select(batch => 
                this.client.SendMessageBatchAsync(new SendMessageBatchRequest
                {
                    QueueUrl = this.queueUrl,
                    Entries = batch.Select(c => new SendMessageBatchRequestEntry
                    {
                        Id = this.textHelpers.GenerateUniqueId(),
                        MessageBody = this.jsonHelpers.ToJsonString(c)
                    }).ToList()
                }
            , cancellationToken));

            return Task.WhenAll(sendMessagesTasks);
        }

        public async Task DeleteMessageAsync(IQueueMessage<T> message, CancellationToken cancellationToken)
        {
            // Make sure this is an AWS queue message.
            var awsMessage = message as AwsQueueMessage<T>;
            if (awsMessage == null)
                return;

            // Remove it from the queue.
            await this.client.DeleteMessageAsync(new DeleteMessageRequest
            {
                QueueUrl = this.queueUrl,
                ReceiptHandle = awsMessage.BaseMessage.ReceiptHandle
            }, cancellationToken);
        }
    }
}