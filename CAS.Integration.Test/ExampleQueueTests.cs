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
        var message = "Hello World";
        var payload = $"{{ \"Message\": \"{message}\" }}";

        // Act
        await this.PublishMessageAsync(payload, _queueDeets.TopicArn);

        // Assert
        await WebAppFactory!.ExampleRepositoryMock.Received(1).CreateAsync(Arg.Is<ExampleModel>(x => x.Message == message), Arg.Any<CancellationToken>());
    }
}