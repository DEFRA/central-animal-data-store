using Amazon.SQS;
using CAS.Infrastructure.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CAS.Core.Consumers;

public class SecondExampleConsumer(
    ILogger<SecondExampleConsumer> logger,
    ISecondExampleRepository secondExampleRepository,
    IAmazonSQS sqsClient,
    IOptions<SecondExampleConsumerOptions> options)
    : QueueConsumerBase<SecondExampleModel>(logger, sqsClient, options)
{
    protected override Task ProcessMessageAsync(SecondExampleModel payload, CancellationToken cancellationToken)
    {
        return secondExampleRepository.CreateAsync(payload, cancellationToken);
    }
}

public record SecondExampleConsumerOptions : QueueConsumerOptions;

// Everything below here would normally be elsewhere in the application

public record SecondExampleModel(string Message);

public interface ISecondExampleRepository
{
    public Task CreateAsync(SecondExampleModel payload, CancellationToken cancellationToken);
}

public class SecondExampleRepository : ISecondExampleRepository
{
    public Task CreateAsync(SecondExampleModel payload, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}