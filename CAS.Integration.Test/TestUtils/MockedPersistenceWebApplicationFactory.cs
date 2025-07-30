using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using CAS.Core.Consumers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            var bob = WebApplication.CreateBuilder();

            var options = bob.Configuration.GetAWSOptions();
            options.Credentials = new BasicAWSCredentials(
                bob.Configuration["AWS_ACCESS_KEY_ID"],
                bob.Configuration["AWS_SECRET_ACCESS_KEY"]
            );

            services.Replace(new ServiceDescriptor(typeof(AWSOptions), options));
            services.AddScoped<IExampleRepository>(x => ExampleRepositoryMock);
            services.AddScoped<ISecondExampleRepository>(x => SecondExampleRepositoryMock);
        });

        return base.CreateHost(builder);
    }
}