using CAS.Core.Consumers;
using CAS.Integration.Test.TestUtils;
using NSubstitute;

namespace CAS.Integration.Test;

public class ExampleQueueTests
    : QueueTestBase, IClassFixture<TestWebApplicationFactory>
{
    private QueueDetails _queueDeets;

    public ExampleQueueTests(TestWebApplicationFactory factory) : base(factory)
    {
        Thread.Sleep(TimeSpan.FromSeconds(20));
        _queueDeets = SetupQueue(factory.Services, "example-queue");
    }
    
    [Fact]
    public async Task MessagePublishToQueue_ShouldBeConsumed()
    {
        // Arrange
        WebAppFactory.ExampleRepositoryMock
            .CreateAsync(Arg.Any<ExampleModel>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        
        var message = "{ \"Message\": \"Hello World\" }";
        var sut = WebAppFactory.CreateClient();
         
        // Act
        await this.PublishMessageAsync(message, _queueDeets.TopicArn);
        
        // Assert
        await WebAppFactory.ExampleRepositoryMock.Received(1).CreateAsync(Arg.Any<ExampleModel>(), Arg.Any<CancellationToken>());
        await WebAppFactory.SecondExampleRepositoryMock.Received(0).CreateAsync(Arg.Any<SecondExampleModel>(), Arg.Any<CancellationToken>());
    }
}