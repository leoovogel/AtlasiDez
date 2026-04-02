using AtlasiDez.Domain.Interfaces;
using AtlasiDez.Infrastructure.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AtlasiDez.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["CityProvider"];

        switch (provider)
        {
            case "BrasilApi":
                services.AddHttpClient<ICityProvider, BrasilApiCityProvider>();
                break;
            case "Ibge":
                services.AddHttpClient<ICityProvider, IbgeCityProvider>();
                break;
            default:
                throw new InvalidOperationException(
                    $"Invalid or missing CityProvider configuration: '{provider}'. Valid values are 'BrasilApi' or 'Ibge'.");
        }

        return services;
    }
}