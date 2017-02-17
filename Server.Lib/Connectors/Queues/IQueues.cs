using Server.Lib.Models.Queues;

namespace Server.Lib.Connectors.Queues
{
    public interface IQueues : IConnector
    {
        IQueue<SendNotification> SendNotifications { get; }
    }
}