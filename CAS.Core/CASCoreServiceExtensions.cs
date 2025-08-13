using Amazon.SimpleNotificationService;
using Amazon.SQS;
using CAS.Core.Consumers;
using CAS.Core.Services;
using CAS.Data;
using CAS.Infrastructure;
using CAS.Infrastructure.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CAS.Core;

public static class CasCoreServiceExtensions
{
    public static IServiceCollection AddCasCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddTransient<ICoreFacade, CoreFacade>();
        services.AddTransient<IAnimalService, AnimalService>();

        services.AddCasData();
        services.AddCasInfrastructure();

        return services;
    }

    public static IServiceCollection AddQueueConsumers(this IServiceCollection services, IConfiguration config)
    {
        services.AddDefaultAWSOptions(config.GetAWSOptions());
        services.AddAWSService<IAmazonSimpleNotificationService>();
        services.AddAWSService<IAmazonSQS>();

        // Add Example Queue Consumer
        services.AddSingleton<IExampleRepository, ExampleRepository>();
        services.AddSingleton<ISecondExampleRepository, SecondExampleRepository>();
        services.AddSingleton<IDisabledExampleRepository, DisabledExampleRepository>();
        services.AddHostedService<ExampleConsumer>()
            .Configure<ExampleConsumerOptions>(config.GetRequiredSection("ExampleQueueConsumer"));
        services.AddHostedService<SecondExampleConsumer>()
            .Configure<SecondExampleConsumerOptions>(config.GetRequiredSection("SecondQueueConsumer"));
        services.AddHostedService<DisabledExampleConsumer>()
            .Configure<DisabledExampleConsumerOptions>(config.GetRequiredSection("DisabledQueueConsumer"));
        return services;
    }
}