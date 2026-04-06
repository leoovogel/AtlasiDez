using Microsoft.AspNetCore.Hosting;

namespace AtlasiDez.Tests.Integration.Fixtures;

public class DevelopmentWebApplicationFactory : AtlasiDezWebApplicationFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment("Development");
    }
}
