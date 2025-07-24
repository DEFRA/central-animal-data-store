using CAS.Core.Consumers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;

namespace CAS.Integration.Test.TestUtils;

public class MockedPersistenceWebApplicationFactory : WebApplicationFactory<Program>
{
    public readonly IExampleRepository ExampleRepositoryMock = Substitute.For<IExampleRepository>();
    public readonly ISecondExampleRepository SecondExampleRepositoryMock = Substitute.For<ISecondExampleRepository>();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTest");
        builder.ConfigureServices(services =>
        {
            services.AddScoped<IExampleRepository>(x => ExampleRepositoryMock);
            services.AddScoped<ISecondExampleRepository>(x => SecondExampleRepositoryMock);
        });

        return base.CreateHost(builder);
    }
}