using System.Diagnostics.CodeAnalysis;
using AtlasiDez.Application.Interfaces;
using AtlasiDez.Domain.Interfaces;
using AtlasiDez.Infrastructure.Cache;
using AtlasiDez.Infrastructure.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AtlasiDez.Infrastructure;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddCityProvider(services, configuration);
        AddCache(services, configuration);

        return services;
    }

    private static void AddCityProvider(IServiceCollection services, IConfiguration configuration)
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
    }

    private static void AddCache(IServiceCollection services, IConfiguration configuration)
    {
        var cacheSection = configuration.GetSection(CacheOptions.SectionName);
        services.Configure<CacheOptions>(cacheSection);

        var cacheOptions = cacheSection.Get<CacheOptions>() ?? new CacheOptions();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = cacheOptions.RedisConnectionString;
            options.InstanceName = "AtlasiDez:";
        });

        services.AddSingleton<ICacheService, RedisCacheService>();
    }
}