using System.Text.Json;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using CAS.Core.Consumers;
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
    public HttpClient? Client { get; private set; }
    public AWSOptions AwsOptions{ get; private set; }
    private readonly WebApplicationBuilder _webApplicationBuilder;

    public MockedPersistenceWebApplicationFactory()
    {
        Console.WriteLine("this was a lambda");
        _webApplicationBuilder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "IntegrationTest"
        });

        var options = _webApplicationBuilder.Configuration.GetAWSOptions();
        options.Credentials = new BasicAWSCredentials(
            _webApplicationBuilder.Configuration["AWS_ACCESS_KEY_ID"],
            _webApplicationBuilder.Configuration["AWS_SECRET_ACCESS_KEY"]
        );

        var serialiserOptions = new JsonSerializerOptions { WriteIndented = true };
        Console.WriteLine($"factory: pulled options [{JsonSerializer.Serialize(options, serialiserOptions)}]");
        ExampleRepositoryMock
            .CreateAsync(Arg.Any<ExampleModel>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        SecondExampleRepositoryMock
            .CreateAsync(Arg.Any<SecondExampleModel>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        
        _webApplicationBuilder.Services.Replace(new ServiceDescriptor(typeof(AWSOptions), options));
        _webApplicationBuilder.Services.AddScoped<IExampleRepository>(x => ExampleRepositoryMock);
        _webApplicationBuilder.Services.AddScoped<ISecondExampleRepository>(x => SecondExampleRepositoryMock);
        _webApplicationBuilder.Services.AddSingleton<IServer, TestServer>();
        
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
        });
        
        var derp = builder.Build();
        derp.Start();
        return derp;
        // var webApp = _webApplicationBuilder.Build();
        // webApp.Start();
        // return webApp;
    }
}