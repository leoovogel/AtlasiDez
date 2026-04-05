using System.Diagnostics.CodeAnalysis;
using AtlasiDez.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace AtlasiDez.Application;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<GetCitiesByStateUseCase>();
        return services;
    }
}