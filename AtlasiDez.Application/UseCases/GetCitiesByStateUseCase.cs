using AtlasiDez.Application.DTOs;
using AtlasiDez.Domain.Entities;
using AtlasiDez.Domain.Interfaces;

namespace AtlasiDez.Application.UseCases;

public class GetCitiesByStateUseCase(ICityProvider cityProvider)
{
    public async Task<PagedResult<City>> ExecuteAsync(string uf, int page = 1, int pageSize = 10)
    {
        var allCities = (await cityProvider.GetByStateAsync(uf)).ToList();

        var items = allCities
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<City>(items, page, pageSize, allCities.Count);
    }
}