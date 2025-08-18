using System.Text.Json;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CAS.Infrastructure.Queues;

public interface IQueuePublisher<T>
{
    Task<AmazonWebServiceResponse> PublishMessage(T payload);
}

public abstract class QueuePublisherBase<T>(
    ILogger<QueuePublisherBase<T>> logger,
    IAmazonSimpleNotificationService snsService,
    IOptions<QueuePublisherOptions> options)
    : IQueuePublisher<T>, IDisposable
{
    private readonly QueuePublisherOptions _queuePublisherOptions = options.Value;

    public async Task<AmazonWebServiceResponse> PublishMessage(T payload)
    {
        logger.LogInformation($"Publishing message to topic: {_queuePublisherOptions.TopicArn}");
        
        var message = JsonSerializer.Serialize(payload);
        var request = new PublishRequest
        {
            Message = message,
            TopicArn = _queuePublisherOptions.TopicArn,
        };

        return await snsService.PublishAsync(request);
    }
    
    public void Dispose()
    {
        snsService.Dispose();
    }
}

public record QueuePublisherOptions
{
    public string TopicArn { get; init; }
}