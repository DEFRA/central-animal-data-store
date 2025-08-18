using CAS.Core.Consumers;
using CAS.Core.Models;
using CAS.Integration.Test.TestUtils;
using NSubstitute;

namespace CAS.Integration.Test.QueueConsumers;

public class ExampleIntegrationTests : IntegrationTestBase
{
    private QueueDetails _queueDeets;

    public ExampleIntegrationTests()
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