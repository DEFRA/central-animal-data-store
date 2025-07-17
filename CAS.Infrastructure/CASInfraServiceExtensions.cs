using Microsoft.Extensions.DependencyInjection;

namespace CAS.Infrastructure;

public static class CASInfraServiceExtensions
{
    public static IServiceCollection AddCasInfrastructure(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        


        return services;
    }
}
