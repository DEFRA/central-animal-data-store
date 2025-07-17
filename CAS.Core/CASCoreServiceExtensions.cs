using CAS.Core.Services;
using CAS.Data;
using CAS.Infrastructure;
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
}
