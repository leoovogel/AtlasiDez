using System.Net.Http.Json;
using AtlasiDez.Application.DTOs;
using AtlasiDez.Domain.Entities;
using AtlasiDez.Tests.Integration.Fixtures;

namespace AtlasiDez.Tests.Integration.Tests;

[Collection("Integration")]
public class CityEndpointCachingTests(AtlasiDezWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetCities_SecondCall_ReturnsCachedData()
    {
        var originalCities = new List<City>
        {
            new("Santa Rosa", "4317202"),
            new("Porto Alegre", "4314902")
        };
        Factory.CityProvider.SetCities("CA", originalCities);

        var firstResponse = await Client.GetAsync("/api/cities/CA");
        var firstResult = await firstResponse.Content.ReadFromJsonAsync<PagedResult<City>>(JsonOptions);

        Factory.CityProvider.SetCities("CA",
        [
            new City("Cidade Diferente", "9999999")
        ]);

        var secondResponse = await Client.GetAsync("/api/cities/CA");
        var secondResult = await secondResponse.Content.ReadFromJsonAsync<PagedResult<City>>(JsonOptions);

        Assert.NotNull(firstResult);
        Assert.NotNull(secondResult);
        Assert.Equal(firstResult.TotalCount, secondResult.TotalCount);
    }

    [Fact]
    public async Task GetCities_CachedData_FilteringStillWorks()
    {
        Factory.CityProvider.SetCities("CB",
        [
            new City("Santa Rosa", "4317202"),
            new City("Santa Maria", "4316907"),
            new City("Porto Alegre", "4314902")
        ]);

        await Client.GetAsync("/api/cities/CB");

        var filteredResponse = await Client.GetAsync("/api/cities/CB?name=Santa");
        var filteredResult = await filteredResponse.Content.ReadFromJsonAsync<PagedResult<City>>(JsonOptions);

        Assert.NotNull(filteredResult);
        Assert.Equal(2, filteredResult.Items.Count);
        Assert.Equal(2, filteredResult.TotalCount);
    }

    [Fact]
    public async Task GetCities_DifferentUfs_CacheIndependently()
    {
        Factory.CityProvider.SetCities("CC", [new City("City CC", "1111111")]);
        Factory.CityProvider.SetCities("CD", [new City("City CD", "2222222")]);

        var responseCC = await Client.GetAsync("/api/cities/CC");
        var resultCC = await responseCC.Content.ReadFromJsonAsync<PagedResult<City>>(JsonOptions);

        var responseCD = await Client.GetAsync("/api/cities/CD");
        var resultCD = await responseCD.Content.ReadFromJsonAsync<PagedResult<City>>(JsonOptions);

        Assert.NotNull(resultCC);
        Assert.NotNull(resultCD);
        Assert.Equal("City CC", resultCC.Items[0].Name);
        Assert.Equal("City CD", resultCD.Items[0].Name);
    }
}
