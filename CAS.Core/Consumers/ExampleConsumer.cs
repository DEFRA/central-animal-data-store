using Amazon.SQS;
using CAS.Core.Models;
using CAS.Infrastructure.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CAS.Core.Consumers;

public class ExampleConsumer(
    ILogger<ExampleConsumer> logger,
    IExampleRepository exampleRepository,
    IAmazonSQS sqsClient,
    IOptions<ExampleConsumerOptions> options)
    : QueueConsumerBase<ExampleModel>(logger, sqsClient, options)
{
    protected override Task ProcessMessageAsync(ExampleModel payload, CancellationToken cancellationToken)
    {
        return exampleRepository.CreateAsync(payload, cancellationToken);
    }
}

public record ExampleConsumerOptions : QueueConsumerOptions;

// Everything below here would normally be elsewhere in the application

public interface IExampleRepository
{
    public Task CreateAsync(ExampleModel payload, CancellationToken cancellationToken);
}

public class ExampleRepository : IExampleRepository
{
    public Task CreateAsync(ExampleModel payload, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}