using AtlasiDez.Application.Interfaces;
using AtlasiDez.Application.UseCases;
using AtlasiDez.Domain.Entities;
using AtlasiDez.Domain.Interfaces;
using NSubstitute;

namespace AtlasiDez.Tests.UseCases;

public class GetCitiesByStateUseCaseTests
{
    private readonly ICityProvider _cityProvider = Substitute.For<ICityProvider>();
    private readonly ICacheService _cacheService = Substitute.For<ICacheService>();
    private readonly GetCitiesByStateUseCase _getCitiesByStateUseCase;

    public GetCitiesByStateUseCaseTests()
    {
        _getCitiesByStateUseCase = new GetCitiesByStateUseCase(_cityProvider, _cacheService);
    }

    [Fact]
    public async Task WhenCacheHit_ReturnsCachedDataWithoutCallingProvider()
    {
        var cachedCities = new List<City>
        {
            new("Boa Vista do Buricá", "4302204"),
            new("Nova Candelária", "4313011"),
            new("Santa Rosa", "4317202"),
            new("Três de Maio", "4321808")
            
        };

        _cacheService.GetAsync<List<City>>("cities:uf:rs")
            .Returns(cachedCities);

        var result = await _getCitiesByStateUseCase.ExecuteAsync("RS", page: 1, pageSize: 10);

        Assert.Equal(4, result.TotalCount);
        Assert.Equal(4, result.Items.Count);
        await _cityProvider.DidNotReceive().GetByStateAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task WhenCacheMiss_FetchesFromProviderAndCaches()
    {
        var cities = new List<City>
        {
            new("Boa Vista do Buricá", "4302204"),
            new("Nova Candelária", "4313011"),
            new("Santa Rosa", "4317202"),
            new("Três de Maio", "4321808")
        };

        _cacheService.GetAsync<List<City>>("cities:uf:rs")
            .Returns((List<City>?)null);

        _cityProvider.GetByStateAsync("RS")
            .Returns(cities);

        var result = await _getCitiesByStateUseCase.ExecuteAsync("RS", page: 1, pageSize: 10);

        Assert.Equal(4, result.TotalCount);
        await _cacheService.Received(1).SetAsync(
            "cities:uf:rs",
            Arg.Any<List<City>>(),
            Arg.Any<TimeSpan?>());
    }

    [Fact]
    public async Task NormalizesCacheKeyToLowercase()
    {
        _cacheService.GetAsync<List<City>>("cities:uf:sc")
            .Returns((List<City>?)null);

        _cityProvider.GetByStateAsync("SC")
            .Returns(new List<City>());

        await _getCitiesByStateUseCase.ExecuteAsync("SC");

        await _cacheService.Received(1).GetAsync<List<City>>("cities:uf:sc");
    }

    [Fact]
    public async Task AppliesPaginationCorrectly()
    {
        var cities = Enumerable.Range(1, 25)
            .Select(i => new City($"City {i}", i.ToString()))
            .ToList();

        _cacheService.GetAsync<List<City>>("cities:uf:pr")
            .Returns(cities);

        var result = await _getCitiesByStateUseCase.ExecuteAsync("PR", page: 2, pageSize: 10);

        Assert.Equal(25, result.TotalCount);
        Assert.Equal(10, result.Items.Count);
        Assert.Equal(2, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal("City 11", result.Items[0].Name);
        Assert.Equal("City 20", result.Items[9].Name);
    }
}
