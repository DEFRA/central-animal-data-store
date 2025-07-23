using CAS.Core.Consumers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;

namespace CAS.Integration.Test.TestUtils;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    public IExampleRepository ExampleRepositoryMock = Substitute.For<IExampleRepository>();
    public ISecondExampleRepository SecondExampleRepositoryMock = Substitute.For<ISecondExampleRepository>();
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTest");
        builder.ConfigureServices(svcs =>
        {
            svcs.AddScoped<IExampleRepository>(x => ExampleRepositoryMock);
            svcs.AddScoped<ISecondExampleRepository>(x => SecondExampleRepositoryMock);
        });
        
        return base.CreateHost(builder);
    }
}