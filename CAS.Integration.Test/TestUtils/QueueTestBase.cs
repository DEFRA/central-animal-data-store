using System.Text.Json;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.DependencyInjection;

namespace CAS.Integration.Test.TestUtils;

[Trait("Dependence", "localstack")]
public class QueueTestBase : IDisposable
{
    protected static MockedPersistenceWebApplicationFactory? WebAppFactory;

    private static IAmazonSQS SqsClient { get; }
    private static IAmazonSimpleNotificationService SnsClient { get; }

    private List<QueueDetails> CreatedQueues { get; } = new();

    static QueueTestBase()
    {
        WebAppFactory ??= new MockedPersistenceWebApplicationFactory();

        var awsOptions = WebAppFactory.AwsOptions;

        if (awsOptions == null)
        {
            throw new NullReferenceException("You must provide AWS Configuration options");
        }

        SqsClient = awsOptions.CreateServiceClient<IAmazonSQS>();
        SnsClient = awsOptions.CreateServiceClient<IAmazonSimpleNotificationService>();
    }

    protected QueueDetails SetupQueue(string queueName)
    {
        var topicReq = new CreateTopicRequest()
        {
            Name = queueName,
        };

        var queueReq = new CreateQueueRequest()
        {
            QueueName = queueName,
        };

        var topicArn = SnsClient.CreateTopicAsync(topicReq).Result.TopicArn;

        var queueUrl = SqsClient.CreateQueueAsync(queueReq).Result.QueueUrl;

        var queueArn = SqsClient.GetQueueAttributesAsync(queueUrl, ["QueueArn"]).Result.QueueARN;

        var subsReq = new SubscribeRequest()
        {
            TopicArn = topicArn,
            Endpoint = queueArn,
            Protocol = "sqs",
            Attributes = new()
            {
                { "RawMessageDelivery", "true"}
            }
        };

        var subscriptionArn = SnsClient.SubscribeAsync(subsReq).Result.SubscriptionArn;
        var deets = new QueueDetails(topicArn, queueUrl, subscriptionArn);
        CreatedQueues.Add(deets);

        return deets;
    }

    protected async Task PublishMessageAsync(string message, string topicArn)
    {
        var request = new PublishRequest
        {
            Message = message,
            TopicArn = topicArn
        };

        var res = await SnsClient.PublishAsync(request);

        // Wait for message poll
        Thread.Sleep(TimeSpan.FromSeconds(3));
    }

    private void TearDownQueues()
    {
        foreach (var queue in CreatedQueues)
        {
            SnsClient.UnsubscribeAsync(queue.SubscriptionArn).Wait();
            SqsClient.DeleteQueueAsync(queue.QueueUrl).Wait();
            SnsClient.DeleteTopicAsync(queue.TopicArn).Wait();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            TearDownQueues();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public record QueueDetails(string TopicArn, string QueueUrl, string SubscriptionArn);
}