using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Common.Helpers;
using Data.Repositories;

namespace Common.Extentions;

public static class DependencyInjection {
    public static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureOptions<DatabaseOptionsHelper>();
        
        services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        
        return services;
    }
}