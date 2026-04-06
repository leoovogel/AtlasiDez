using System.Text.Json;

namespace AtlasiDez.Tests.Integration.Fixtures;

[Collection("Integration")]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly HttpClient Client;
    protected readonly AtlasiDezWebApplicationFactory Factory;

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    protected IntegrationTestBase(AtlasiDezWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        Factory.CityProvider.Reset();
        return Task.CompletedTask;
    }
}
