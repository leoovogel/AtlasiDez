using AtlasiDez.Api.Controllers;
using AtlasiDez.Application.DTOs;
using AtlasiDez.Application.Interfaces;
using AtlasiDez.Application.UseCases;
using AtlasiDez.Domain.Entities;
using AtlasiDez.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace AtlasiDez.Tests.Controllers;

public class CityControllerTests
{
    private readonly ICityProvider _cityProvider = Substitute.For<ICityProvider>();
    private readonly ICacheService _cacheService = Substitute.For<ICacheService>();
    private readonly CityController _controller;

    public CityControllerTests()
    {
        var useCase = new GetCitiesByStateUseCase(_cityProvider, _cacheService);
        _controller = new CityController(useCase);
    }

    [Fact]
    public async Task WhenValidUf_ReturnsOkWithPagedResult()
    {
        var cities = new List<City>
        {
            new("Boa Vista do Buricá", "4302204"),
            new("Nova Candelária", "4313011"),
            new("Santa Rosa", "4317202"),
            new("Três de Maio", "4321808")
        };

        _cacheService.GetAsync<List<City>>("cities:uf:rs")
            .Returns(cities);

        var result = await _controller.GetByState("RS");

        var okResult = Assert.IsType<OkObjectResult>(result);
        var pagedResult = Assert.IsType<PagedResult<City>>(okResult.Value);
        Assert.Equal(5, pagedResult.TotalCount);
        Assert.Equal(4, pagedResult.Items.Count);
    }

    [Fact]
    public async Task WhenValidUfWithPagination_ReturnsCorrectPage()
    {
        var cities = Enumerable.Range(1, 25)
            .Select(i => new City($"City {i}", i.ToString()))
            .ToList();

        _cacheService.GetAsync<List<City>>("cities:uf:sc")
            .Returns(cities);

        var result = await _controller.GetByState("SC", page: 2, pageSize: 10);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var pagedResult = Assert.IsType<PagedResult<City>>(okResult.Value);
        Assert.Equal(25, pagedResult.TotalCount);
        Assert.Equal(10, pagedResult.Items.Count);
        Assert.Equal(2, pagedResult.Page);
        Assert.Equal("City 11", pagedResult.Items[0].Name);
    }

    [Fact]
    public async Task WhenUseCaseReturnsEmptyList_ReturnsOkWithEmptyItems()
    {
        _cacheService.GetAsync<List<City>>("cities:uf:xx")
            .Returns((List<City>?)null);

        _cityProvider.GetByStateAsync("XX")
            .Returns(Enumerable.Empty<City>());

        var result = await _controller.GetByState("XX");

        var okResult = Assert.IsType<OkObjectResult>(result);
        var pagedResult = Assert.IsType<PagedResult<City>>(okResult.Value);
        Assert.Equal(0, pagedResult.TotalCount);
        Assert.Empty(pagedResult.Items);
    }

    [Fact]
    public async Task WhenDefaultParameters_UsesPageOneAndPageSizeTen()
    {
        var cities = Enumerable.Range(1, 15)
            .Select(i => new City($"City {i}", i.ToString()))
            .ToList();

        _cacheService.GetAsync<List<City>>("cities:uf:rs")
            .Returns(cities);

        var result = await _controller.GetByState("RS");

        var okResult = Assert.IsType<OkObjectResult>(result);
        var pagedResult = Assert.IsType<PagedResult<City>>(okResult.Value);
        Assert.Equal(1, pagedResult.Page);
        Assert.Equal(10, pagedResult.PageSize);
        Assert.Equal(10, pagedResult.Items.Count);
    }
}
