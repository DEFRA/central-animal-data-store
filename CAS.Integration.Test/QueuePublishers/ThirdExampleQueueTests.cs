using CAS.Core.Consumers;
using CAS.Core.Models;
using CAS.Integration.Test.TestUtils;
using FluentAssertions;
using NSubstitute;

namespace CAS.Integration.Test.QueuePublishers;

public class ExamplePublisherTests : IntegrationTestBase
{
    private QueueDetails _queueDeets;

    public ExamplePublisherTests()
    {
        _queueDeets = SetupQueue("example-publisher-queue");
    }

    [Fact]
    public async Task MessagePublishToQueue_ShouldPublishToTheQueue()
    {
        // Arrange
        var message = "Hello Sunshine";
        var payload = new ExampleModel(message);

        // Act
        await PostMessage<ExampleModel>(payload, "/api/enqueue");
        var queueResponse = await ReceiveMessageAsync<ExampleModel>(_queueDeets.QueueUrl);

        // Assert
        queueResponse.Should().NotBeNull();
        queueResponse.Message.Should().Be(message);
    }
}