using System.Text.Json;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using CAS.Core.Consumers;
using CAS.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
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
    public readonly IDisabledExampleRepository DisabledExampleRepositoryMock = Substitute.For<IDisabledExampleRepository>();
    public HttpClient? Client { get; private set; }
    public AWSOptions AwsOptions { get; private set; }

    public MockedPersistenceWebApplicationFactory()
    {
        var webApplicationBuilder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "IntegrationTest"
        });

        var options = webApplicationBuilder.Configuration.GetAWSOptions();
        options.Credentials = new BasicAWSCredentials(
            webApplicationBuilder.Configuration["AWS_ACCESS_KEY_ID"],
            webApplicationBuilder.Configuration["AWS_SECRET_ACCESS_KEY"]
        );

        ExampleRepositoryMock
            .CreateAsync(Arg.Any<ExampleModel>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        SecondExampleRepositoryMock
            .CreateAsync(Arg.Any<SecondExampleModel>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        DisabledExampleRepositoryMock
            .CreateAsync(Arg.Any<DisabledExampleModel>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        webApplicationBuilder.Services.Replace(new ServiceDescriptor(typeof(AWSOptions), options));
        webApplicationBuilder.Services.AddScoped<IExampleRepository>(x => ExampleRepositoryMock);
        webApplicationBuilder.Services.AddScoped<ISecondExampleRepository>(x => SecondExampleRepositoryMock);
        webApplicationBuilder.Services.AddScoped<IDisabledExampleRepository>(x => DisabledExampleRepositoryMock);
        webApplicationBuilder.Services.AddSingleton<IServer, TestServer>();

        AwsOptions = options;
        Client ??= CreateClient();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.Replace(new ServiceDescriptor(typeof(AWSOptions), AwsOptions));
            services.AddSingleton<IExampleRepository>(x => ExampleRepositoryMock);
            services.AddSingleton<ISecondExampleRepository>(x => SecondExampleRepositoryMock);
            services.AddSingleton<IDisabledExampleRepository>(x => DisabledExampleRepositoryMock);
        });
        var app = builder.Build();
        app.Start();
        return app;
    }
}