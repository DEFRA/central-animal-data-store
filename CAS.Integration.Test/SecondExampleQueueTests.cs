using CAS.Core.Consumers;
using CAS.Integration.Test.TestUtils;
using NSubstitute;

namespace CAS.Integration.Test;

public class SecondExampleQueueTests
    : QueueTestBase
{
    private QueueDetails _queueDeets;

    public SecondExampleQueueTests() : base()
    {
        _queueDeets = SetupQueue(WebAppFactory.Services, "second-queue");
    }

    [Fact]
    public async Task MessagePublishToQueue_ShouldBeConsumed()
    {
        // Arrange
        WebAppFactory.SecondExampleRepositoryMock
            .CreateAsync(Arg.Any<SecondExampleModel>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var message = "{ \"Message\": \"Hello Kitty\" }";
        var sut = WebAppFactory.CreateClient();

        // Act
        await this.PublishMessageAsync(message, _queueDeets.TopicArn);

        // Assert
        await WebAppFactory.SecondExampleRepositoryMock.Received(1).CreateAsync(Arg.Any<SecondExampleModel>(), Arg.Any<CancellationToken>());
    }
}