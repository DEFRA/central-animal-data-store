using CAS.Core.Consumers;
using CAS.Integration.Test.TestUtils;
using NSubstitute;

namespace CAS.Integration.Test.QueueConsumers;

public class DisabledExampleIntegrationTests : IntegrationTestBase
{
    private QueueDetails _queueDeets;

    public DisabledExampleIntegrationTests()
    {
        _queueDeets = SetupQueue("disabled-consumer-queue");
    }

    [Fact]
    public async Task MessagePublishToQueue_ShouldBeConsumed()
    {
        // Arrange
        var message = "{ \"Message\": \"Hello! Is it me you're looking for?\" }";

        // Act
        await this.PublishMessageAsync(message, _queueDeets.TopicArn);

        // Assert
        await WebAppFactory!.DisabledExampleRepositoryMock.Received(0).CreateAsync(Arg.Any<DisabledExampleModel>(), Arg.Any<CancellationToken>());
    }
}