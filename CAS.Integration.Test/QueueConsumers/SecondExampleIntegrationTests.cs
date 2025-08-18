using CAS.Core.Consumers;
using CAS.Integration.Test.TestUtils;
using NSubstitute;

namespace CAS.Integration.Test.QueueConsumers;

public class SecondExampleIntegrationTests : IntegrationTestBase
{
    private QueueDetails _queueDeets;

    public SecondExampleIntegrationTests()
    {
        _queueDeets = SetupQueue("second-queue");
    }

    [Fact]
    public async Task MessagePublishToQueue_ShouldBeConsumed()
    {
        // Arrange
        var message = "Hello Kitty";
        var payload = $"{{ \"DifferentShapedMessage\": \"{message}\" }}";

        // Act
        await this.PublishMessageAsync(payload, _queueDeets.TopicArn);

        // Assert
        await WebAppFactory!.SecondExampleRepositoryMock.Received(1).CreateAsync(Arg.Is<SecondExampleModel>(x => x.DifferentShapedMessage == message), Arg.Any<CancellationToken>());
    }
}