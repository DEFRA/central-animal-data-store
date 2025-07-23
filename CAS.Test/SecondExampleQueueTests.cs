using CAS.Core.Consumers;
using CAS.Test.TestUtils;
using NSubstitute;

namespace CAS.Test;

public class SecondExampleQueueTests
    : QueueTestBase, IClassFixture<TestWebApplicationFactory>
{
    private QueueDetails _queueDeets;

    public SecondExampleQueueTests(TestWebApplicationFactory factory) : base(factory)
    {
        _queueDeets = SetupQueue(factory.Services, "second-queue");
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
        await WebAppFactory.ExampleRepositoryMock.Received(0).CreateAsync(Arg.Any<ExampleModel>(), Arg.Any<CancellationToken>());
    }
}