using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Common.Helpers;
using Data.Repositories;
using PaymentApi.Endpoints;
using PaymentApi.Services;

namespace PaymentApi.Extentions;

public static class DependencyInjection {
    public static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureOptions<DatabaseOptionsHelper>();
        
        services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddTransient<PaymeService>();
        services.AddTransient<ClickService>();
        services.AddTransient<ChequeService>();
        return services;
    }
}