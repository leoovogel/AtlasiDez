using AtlasiDez.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace AtlasiDez.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<GetCitiesByStateUseCase>();
        return services;
    }
}