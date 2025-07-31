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
    public AWSOptions AWSOptions{ get; private set; }
    private WebApplicationBuilder _webApplicationBuilder;

    public MockedPersistenceWebApplicationFactory()
    {
        Console.WriteLine("this was a lambda");
        _webApplicationBuilder = WebApplication.CreateBuilder();

        var options = _webApplicationBuilder.Configuration.GetAWSOptions();
        options.Credentials = new BasicAWSCredentials(
            _webApplicationBuilder.Configuration["AWS_ACCESS_KEY_ID"],
            _webApplicationBuilder.Configuration["AWS_SECRET_ACCESS_KEY"]
        );

        _webApplicationBuilder.Services.Replace(new ServiceDescriptor(typeof(AWSOptions), options));
        _webApplicationBuilder.Services.AddScoped<IExampleRepository>(x => ExampleRepositoryMock);
        _webApplicationBuilder.Services.AddScoped<ISecondExampleRepository>(x => SecondExampleRepositoryMock);
        _webApplicationBuilder.Services.AddSingleton<IServer, TestServer>();
        
        AWSOptions = options;
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var webApp = _webApplicationBuilder.Build();
        webApp.Start();
        return webApp;
        //
        // Console.WriteLine("this is a factory");
        // builder.UseEnvironment("IntegrationTest");

        // builder.ConfigureServices(services =>
        // {
        //     Console.WriteLine("this is a lambda");
        //     var bob = WebApplication.CreateBuilder();
        //
        //     var options = bob.Configuration.GetAWSOptions();
        //     options.Credentials = new BasicAWSCredentials(
        //         bob.Configuration["AWS_ACCESS_KEY_ID"],
        //         bob.Configuration["AWS_SECRET_ACCESS_KEY"]
        //     );
        //
        //     services.Replace(new ServiceDescriptor(typeof(AWSOptions), options));
        //     services.AddScoped<IExampleRepository>(x => ExampleRepositoryMock);
        //     services.AddScoped<ISecondExampleRepository>(x => SecondExampleRepositoryMock);
        //
        //     bob.Build();
        // });
        //
        // Console.WriteLine("this was a lambda");
        // var bob = WebApplication.CreateBuilder();
        //
        // var options = bob.Configuration.GetAWSOptions();
        // options.Credentials = new BasicAWSCredentials(
        //     bob.Configuration["AWS_ACCESS_KEY_ID"],
        //     bob.Configuration["AWS_SECRET_ACCESS_KEY"]
        // );
        //
        // bob.Services.Replace(new ServiceDescriptor(typeof(AWSOptions), options));
        // bob.Services.AddScoped<IExampleRepository>(x => ExampleRepositoryMock);
        // bob.Services.AddScoped<ISecondExampleRepository>(x => SecondExampleRepositoryMock);
        // bob.Services.AddSingleton<IServer, TestServer>();
        //
        // AWSOptions = options;
        //
        // var webApp = bob.Build();
        // webApp.Start();
        //
        // return webApp;
    }
}