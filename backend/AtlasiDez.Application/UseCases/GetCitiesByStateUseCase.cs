using AtlasiDez.Application.DTOs;
using AtlasiDez.Application.Interfaces;
using AtlasiDez.Domain.Entities;
using AtlasiDez.Domain.Interfaces;

namespace AtlasiDez.Application.UseCases;

public class GetCitiesByStateUseCase(
    ICityProvider cityProvider,
    ICacheService cacheService)
{
    public async Task<PagedResult<City>> ExecuteAsync(string uf, int page = 1, int pageSize = 10, string? name = null)
    {
        var cacheKey = $"cities:uf:{uf.ToLowerInvariant()}";

        var cachedCities = await cacheService.GetAsync<List<City>>(cacheKey);

        if (cachedCities is not null)
            return Paginate(cachedCities, page, pageSize, name);

        var allCities = (await cityProvider.GetByStateAsync(uf)).ToList();

        await cacheService.SetAsync(cacheKey, allCities);

        return Paginate(allCities, page, pageSize, name);
    }

    private static PagedResult<City> Paginate(List<City> cities, int page, int pageSize, string? name = null)
    {
        var filtered = string.IsNullOrWhiteSpace(name)
            ? cities
            : cities.Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();

        var items = filtered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<City>(items, page, pageSize, filtered.Count);
    }
}