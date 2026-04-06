using System.Net;
using System.Text.Json;
using AtlasiDez.Tests.Integration.Fixtures;

namespace AtlasiDez.Tests.Integration.Tests;

public class CityEndpointDevelopmentEnvironmentTests : IClassFixture<DevelopmentWebApplicationFactory>, IAsyncLifetime
{
    private readonly DevelopmentWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CityEndpointDevelopmentEnvironmentTests(DevelopmentWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCities_ErrorInDevelopment_IncludesStackTrace()
    {
        _factory.CityProvider.SetException(new InvalidOperationException("erro dev"));

        var response = await _client.GetAsync("/api/cities/DA");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        var hasStackTrace = doc.RootElement.TryGetProperty("stackTrace", out _) || doc.RootElement.TryGetProperty("stack_trace", out _);
        Assert.True(hasStackTrace);
    }

    [Fact]
    public async Task GetCities_ErrorInDevelopment_DetailIsExceptionMessage()
    {
        _factory.CityProvider.SetException(new InvalidOperationException("mensagem real do erro"));

        var response = await _client.GetAsync("/api/cities/DB");

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        var detail = doc.RootElement.GetProperty("detail").GetString();
        Assert.Equal("mensagem real do erro", detail);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        _factory.CityProvider.Reset();
        return Task.CompletedTask;
    }
}
