using AtlasiDez.Domain.Interfaces;
using AtlasiDez.Tests.Integration.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AtlasiDez.Tests.Integration.Fixtures;

public class AtlasiDezWebApplicationFactory : WebApplicationFactory<Program>
{
    public FakeCityProvider CityProvider { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Production");

        builder.UseSetting("CityProvider", "Ibge");
        builder.UseSetting("Cache:RedisConnectionString", "localhost:6379");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ICityProvider>();
            services.AddSingleton<ICityProvider>(CityProvider);

            services.RemoveAll<IDistributedCache>();
            services.AddDistributedMemoryCache();
        });
    }
}
