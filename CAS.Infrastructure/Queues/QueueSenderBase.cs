using Amazon.SimpleNotificationService;

namespace CAS.Infrastructure.Queues;

public interface IQueueSender
{
    void SendMessage(string message);
}

public abstract class QueueSenderBase(IAmazonSimpleNotificationService snsService) : IQueueSender
{
    public abstract void SendMessage(string message);
}