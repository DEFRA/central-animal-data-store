using Amazon.SimpleNotificationService;

namespace CAS.Infrastructure.Queues;

public interface IQueuePublisher
{
    void PublishMessage(string message);
}

public abstract class QueuePublisherBase(IAmazonSimpleNotificationService snsService) : IQueuePublisher
{
    public abstract void PublishMessage(string message);
}