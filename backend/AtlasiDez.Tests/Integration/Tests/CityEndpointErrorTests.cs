using System.Net;
using System.Text.Json;
using AtlasiDez.Tests.Integration.Fixtures;
using Microsoft.AspNetCore.Mvc;

namespace AtlasiDez.Tests.Integration.Tests;

[Collection("Integration")]
public class CityEndpointErrorTests(AtlasiDezWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetCities_ProviderThrowsHttpRequestException_Returns502()
    {
        Factory.CityProvider.SetException(new HttpRequestException("connection refused"));

        var response = await Client.GetAsync("/api/cities/EA");

        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);

        var problemDetails = await DeserializeProblemDetails(response);
        Assert.Equal("Bad Gateway", problemDetails.Title);
    }

    [Fact]
    public async Task GetCities_ProviderThrowsArgumentException_Returns400()
    {
        Factory.CityProvider.SetException(new ArgumentException("UF invalido"));

        var response = await Client.GetAsync("/api/cities/EB");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await DeserializeProblemDetails(response);
        Assert.Equal("Bad Request", problemDetails.Title);
    }

    [Fact]
    public async Task GetCities_ProviderThrowsUnexpectedException_Returns500()
    {
        Factory.CityProvider.SetException(new InvalidOperationException("algo quebrou"));

        var response = await Client.GetAsync("/api/cities/EC");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var problemDetails = await DeserializeProblemDetails(response);
        Assert.Equal("Internal Server Error", problemDetails.Title);
        Assert.Equal("Ocorreu um erro interno no servidor", problemDetails.Detail);
    }

    [Fact]
    public async Task GetCities_ErrorInProduction_DoesNotExposeStackTrace()
    {
        Factory.CityProvider.SetException(new InvalidOperationException("secret detail"));

        var response = await Client.GetAsync("/api/cities/EE");

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        Assert.False(doc.RootElement.TryGetProperty("stackTrace", out _));
        Assert.False(doc.RootElement.TryGetProperty("stack_trace", out _));
    }

    private static async Task<ProblemDetails> DeserializeProblemDetails(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return problemDetails!;
    }
}
