namespace Server.Lib.Connectors.Queues
{
    public interface IQueueMessage<out T> where T : QueueMessageContent
    {
        T Content { get; }
        int DequeueCount { get; }
    }
}