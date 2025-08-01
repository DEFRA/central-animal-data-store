using CAS.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CAS.Data;

public static class CasDataServiceExtensions
{
    public static IServiceCollection AddCasData(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddTransient<IAnimalRepository, AnimalRepository>();


        return services;
    }
}