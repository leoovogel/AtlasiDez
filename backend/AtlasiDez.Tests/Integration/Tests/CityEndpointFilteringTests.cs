using System.Net.Http.Json;
using AtlasiDez.Application.DTOs;
using AtlasiDez.Domain.Entities;
using AtlasiDez.Tests.Integration.Fixtures;

namespace AtlasiDez.Tests.Integration.Tests;

[Collection("Integration")]
public class CityEndpointFilteringTests(AtlasiDezWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetCities_NameFilter_ReturnsMatchingCities()
    {
        Factory.CityProvider.SetCities("FA",
        [
            new City("Santa Rosa", "4317202"),
            new City("Santa Maria", "4316907"),
            new City("Porto Alegre", "4314902")
        ]);

        var response = await Client.GetAsync("/api/cities/FA?name=Santa");
        var result = await response.Content.ReadFromJsonAsync<PagedResult<City>>(JsonOptions);

        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task GetCities_NameFilter_IsCaseInsensitive()
    {
        Factory.CityProvider.SetCities("FB",
        [
            new City("Santa Rosa", "4317202"),
            new City("Santa Maria", "4316907"),
            new City("Porto Alegre", "4314902")
        ]);

        var response = await Client.GetAsync("/api/cities/FB?name=santa");
        var result = await response.Content.ReadFromJsonAsync<PagedResult<City>>(JsonOptions);

        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetCities_NameFilter_PartialMatch()
    {
        Factory.CityProvider.SetCities("FC",
        [
            new City("Porto Alegre", "4314902"),
            new City("Alegrete", "4300604"),
            new City("Santa Rosa", "4317202")
        ]);

        var response = await Client.GetAsync("/api/cities/FC?name=Alegre");
        var result = await response.Content.ReadFromJsonAsync<PagedResult<City>>(JsonOptions);

        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetCities_NameFilter_NoMatch_ReturnsEmpty()
    {
        Factory.CityProvider.SetCities("FD",
        [
            new City("Santa Rosa", "4317202"),
            new City("Porto Alegre", "4314902")
        ]);

        var response = await Client.GetAsync("/api/cities/FD?name=XYZ");
        var result = await response.Content.ReadFromJsonAsync<PagedResult<City>>(JsonOptions);

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetCities_NameFilter_WithPagination_PaginatesFilteredResults()
    {
        var cities = Enumerable.Range(1, 15)
            .Select(i => new City($"Santa City {i}", i.ToString()))
            .Concat([new City("Porto Alegre", "999")])
            .ToList();
        Factory.CityProvider.SetCities("FE", cities);

        var response = await Client.GetAsync("/api/cities/FE?name=Santa&page=2&pageSize=5");
        var result = await response.Content.ReadFromJsonAsync<PagedResult<City>>(JsonOptions);

        Assert.NotNull(result);
        Assert.Equal(5, result.Items.Count);
        Assert.Equal(15, result.TotalCount);
        Assert.Equal("Santa City 6", result.Items[0].Name);
    }

    [Fact]
    public async Task GetCities_NameFilter_TotalCountReflectsFilteredCount()
    {
        var cities = Enumerable.Range(1, 20)
            .Select(i => i <= 5
                ? new City($"Santa City {i}", i.ToString())
                : new City($"Other City {i}", i.ToString()))
            .ToList();
        Factory.CityProvider.SetCities("FF", cities);

        var response = await Client.GetAsync("/api/cities/FF?name=Santa");
        var result = await response.Content.ReadFromJsonAsync<PagedResult<City>>(JsonOptions);

        Assert.NotNull(result);
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(5, result.Items.Count);
    }
}
