using CAS.Core.Consumers;
using CAS.Integration.Test.TestUtils;
using NSubstitute;

namespace CAS.Integration.Test;

public class SecondExampleQueueTests : QueueTestBase
{
    private QueueDetails _queueDeets;

    public SecondExampleQueueTests() : base()
    {
        _queueDeets = SetupQueue("second-queue");
    }

    [Fact]
    public async Task MessagePublishToQueue_ShouldBeConsumed()
    {
        // Arrange
        var message = "{ \"Message\": \"Hello Kitty\" }";

        // Act
        await this.PublishMessageAsync(message, _queueDeets.TopicArn);

        // Assert
        await WebAppFactory!.SecondExampleRepositoryMock.Received(1).CreateAsync(Arg.Any<SecondExampleModel>(), Arg.Any<CancellationToken>());
    }
}