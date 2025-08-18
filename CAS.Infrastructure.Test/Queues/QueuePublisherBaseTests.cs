using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using CAS.Infrastructure.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace CAS.Infrastructure.Test.Queues;

public class QueuePublisherBaseTests
{
    [Fact]
    public void Dispose_ShouldCallDisposeOnSnsService()
    {
        // Arrange
        var mockSetup = CreateMocks();

        // Act
        using (var sut = new PublisherBaseTestHarness(mockSetup));

        // Assert
        mockSetup.SnsService.Received(1).Dispose();
    }

    [Fact]
    public async Task PublishAsync_ShouldCallPublishOnSnsService()
    {
        // Arrange
        var mockSetup = CreateMocks();
        var sut = new PublisherBaseTestHarness(mockSetup);
        var payload = new TestModel("Test", 12);
        
        // Act
        await sut.PublishMessage(payload);

        // Assert
        await mockSetup.SnsService.Received(1).PublishAsync(Arg.Any<PublishRequest>());
    }
    
    public record TestModel(string TestString, int TestInt);

    public record HarnessSetup(
        ILogger<QueuePublisherBase<TestModel>> Logger,
        IAmazonSimpleNotificationService SnsService,
        IOptions<QueuePublisherOptions> Options);
    
    private static HarnessSetup CreateMocks()
    {
        var logger = Substitute.For<ILogger<QueuePublisherBase<TestModel>>>();
        var snsService = Substitute.For<IAmazonSimpleNotificationService>();
        var options = Substitute.For<IOptions<QueuePublisherOptions>>();
        options.Value.Returns(new QueuePublisherOptions()
        {
            TopicArn = "testTopic"
        });

        return new HarnessSetup(logger, snsService, options);
    }

    private class PublisherBaseTestHarness(HarnessSetup setup)
        : QueuePublisherBase<TestModel>(setup.Logger, setup.SnsService, setup.Options);
}