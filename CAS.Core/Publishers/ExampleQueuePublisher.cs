using Amazon.SimpleNotificationService;
using CAS.Core.Models;
using CAS.Infrastructure.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CAS.Core.Publishers;

public class ExampleQueuePublisher(
    ILogger<QueuePublisherBase<ExampleModel>> logger,
    IAmazonSimpleNotificationService snsService,
    IOptions<ExamplePublisherOptions> options) :
    QueuePublisherBase<ExampleModel>(logger, snsService, options);

public record ExamplePublisherOptions : QueuePublisherOptions;