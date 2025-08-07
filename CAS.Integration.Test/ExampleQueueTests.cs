using CAS.Core.Consumers;
using CAS.Integration.Test.TestUtils;
using NSubstitute;

namespace CAS.Integration.Test;

public class ExampleQueueTests : QueueTestBase
{
    private QueueDetails _queueDeets;

    public ExampleQueueTests()
    {
        _queueDeets = SetupQueue("example-queue");
    }

    [Fact]
    public async Task MessagePublishToQueue_ShouldBeConsumed()
    {
        // Arrange
        var message = "{ \"Message\": \"Hello World\" }";

        // Act
        await this.PublishMessageAsync(message, _queueDeets.TopicArn);

        // Assert
        await WebAppFactory!.ExampleRepositoryMock.Received(1).CreateAsync(Arg.Any<ExampleModel>(), Arg.Any<CancellationToken>());
    }
}