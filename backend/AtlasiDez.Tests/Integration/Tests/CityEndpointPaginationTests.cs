using System.Net.Http.Json;
using AtlasiDez.Application.DTOs;
using AtlasiDez.Domain.Entities;
using AtlasiDez.Tests.Integration.Fixtures;

namespace AtlasiDez.Tests.Integration.Tests;

[Collection("Integration")]
public class CityEndpointPaginationTests(AtlasiDezWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetCities_Page2_ReturnsCorrectSlice()
    {
        var cities = Enumerable.Range(1, 25)
            .Select(i => new City($"City {i}", i.ToString()))
            .ToList();
        Factory.CityProvider.SetCities("PA", cities);

        var response = await Client.GetAsync("/api/cities/PA?page=2&pageSize=10");
        var result = await response.Content.ReadFromJsonAsync<PagedResult<City>>(JsonOptions);

        Assert.NotNull(result);
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(10, result.Items.Count);
        Assert.Equal(2, result.Page);
        Assert.Equal("City 11", result.Items[0].Name);
    }

    [Fact]
    public async Task GetCities_LastPage_ReturnsRemainingItems()
    {
        var cities = Enumerable.Range(1, 25)
            .Select(i => new City($"City {i}", i.ToString()))
            .ToList();
        Factory.CityProvider.SetCities("PB", cities);

        var response = await Client.GetAsync("/api/cities/PB?page=3&pageSize=10");
        var result = await response.Content.ReadFromJsonAsync<PagedResult<City>>(JsonOptions);

        Assert.NotNull(result);
        Assert.Equal(5, result.Items.Count);
        Assert.Equal(25, result.TotalCount);
        Assert.Equal("City 21", result.Items[0].Name);
        Assert.Equal("City 25", result.Items[4].Name);
    }

    [Fact]
    public async Task GetCities_PageBeyondData_ReturnsEmptyItems()
    {
        var cities = Enumerable.Range(1, 5)
            .Select(i => new City($"City {i}", i.ToString()))
            .ToList();
        Factory.CityProvider.SetCities("PC", cities);

        var response = await Client.GetAsync("/api/cities/PC?page=2&pageSize=10");
        var result = await response.Content.ReadFromJsonAsync<PagedResult<City>>(JsonOptions);

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(5, result.TotalCount);
    }

    [Fact]
    public async Task GetCities_CustomPageSize_RespectsPageSize()
    {
        var cities = Enumerable.Range(1, 20)
            .Select(i => new City($"City {i}", i.ToString()))
            .ToList();
        Factory.CityProvider.SetCities("PD", cities);

        var response = await Client.GetAsync("/api/cities/PD?pageSize=5");
        var result = await response.Content.ReadFromJsonAsync<PagedResult<City>>(JsonOptions);

        Assert.NotNull(result);
        Assert.Equal(5, result.Items.Count);
        Assert.Equal(5, result.PageSize);
    }

    [Fact]
    public async Task GetCities_PageSizeLargerThanData_ReturnsAllItems()
    {
        var cities = Enumerable.Range(1, 3)
            .Select(i => new City($"City {i}", i.ToString()))
            .ToList();
        Factory.CityProvider.SetCities("PE", cities);

        var response = await Client.GetAsync("/api/cities/PE?pageSize=100");
        var result = await response.Content.ReadFromJsonAsync<PagedResult<City>>(JsonOptions);

        Assert.NotNull(result);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
    }
}
