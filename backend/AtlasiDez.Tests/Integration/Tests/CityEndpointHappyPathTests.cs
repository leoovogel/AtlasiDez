using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AtlasiDez.Application.DTOs;
using AtlasiDez.Domain.Entities;
using AtlasiDez.Tests.Integration.Fixtures;

namespace AtlasiDez.Tests.Integration.Tests;

[Collection("Integration")]
public class CityEndpointHappyPathTests(AtlasiDezWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetCities_ValidUf_Returns200Ok()
    {
        Factory.CityProvider.SetCities("HP", [new City("Santa Rosa", "4317202")]);

        var response = await Client.GetAsync("/api/cities/HP");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetCities_ValidUf_ReturnsCorrectCities()
    {
        var cities = new List<City>
        {
            new("Boa Vista do Buricá", "4302204"),
            new("Nova Candelária", "4313011"),
            new("Santa Rosa", "4317202")
        };
        Factory.CityProvider.SetCities("HC", cities);

        var response = await Client.GetAsync("/api/cities/HC");
        var result = await response.Content.ReadFromJsonAsync<PagedResult<City>>(JsonOptions);

        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal("Boa Vista do Buricá", result.Items[0].Name);
        Assert.Equal("4302204", result.Items[0].IbgeCode);
    }

    [Fact]
    public async Task GetCities_DefaultPagination_ReturnsPage1With10Items()
    {
        var cities = Enumerable.Range(1, 15)
            .Select(i => new City($"City {i}", i.ToString()))
            .ToList();
        Factory.CityProvider.SetCities("HD", cities);

        var response = await Client.GetAsync("/api/cities/HD");
        var result = await response.Content.ReadFromJsonAsync<PagedResult<City>>(JsonOptions);

        Assert.NotNull(result);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(10, result.Items.Count);
        Assert.Equal(15, result.TotalCount);
    }

    [Fact]
    public async Task GetCities_EmptyState_Returns200WithEmptyItems()
    {
        var response = await Client.GetAsync("/api/cities/HE");
        var result = await response.Content.ReadFromJsonAsync<PagedResult<City>>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetCities_ResponseBody_UsesSnakeCaseNaming()
    {
        Factory.CityProvider.SetCities("HJ", [new City("Teste", "1234567")]);

        var response = await Client.GetAsync("/api/cities/HJ");
        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.True(root.TryGetProperty("items", out _));
        Assert.True(root.TryGetProperty("page", out _));
        Assert.True(root.TryGetProperty("page_size", out _));
        Assert.True(root.TryGetProperty("total_count", out _));

        var firstItem = root.GetProperty("items").EnumerateArray().First();
        Assert.True(firstItem.TryGetProperty("name", out _));
        Assert.True(firstItem.TryGetProperty("ibge_code", out _));
    }
}
