using Amazon.SQS;
using CAS.Infrastructure.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CAS.Core.Consumers;

public class DisabledExampleConsumer(
    ILogger<DisabledExampleConsumer> logger,
    IDisabledExampleRepository DisabledExampleRepository,
    IAmazonSQS sqsClient,
    IOptions<DisabledExampleConsumerOptions> options)
    : QueueConsumerBase<DisabledExampleModel>(logger, sqsClient, options)
{
    protected override Task ProcessMessageAsync(DisabledExampleModel payload, CancellationToken cancellationToken)
    {
        return DisabledExampleRepository.CreateAsync(payload, cancellationToken);
    }
}

public record DisabledExampleConsumerOptions : QueueConsumerOptions;

// Everything below here would normally be elsewhere in the application

public record DisabledExampleModel(string Message);

public interface IDisabledExampleRepository
{
    public Task CreateAsync(DisabledExampleModel payload, CancellationToken cancellationToken);
}

public class DisabledExampleRepository : IDisabledExampleRepository
{
    public Task CreateAsync(DisabledExampleModel payload, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}