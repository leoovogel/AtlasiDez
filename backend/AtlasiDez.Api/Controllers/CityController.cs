using AtlasiDez.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace AtlasiDez.Api.Controllers;

[ApiController]
[Route("api/cities")]
public class CityController(GetCitiesByStateUseCase useCase) : ControllerBase
{
    [HttpGet("{uf}")]
    public async Task<IActionResult> GetByState(string uf, int page = 1, int pageSize = 10, string? name = null)
    {
        var result = await useCase.ExecuteAsync(uf, page, pageSize, name);
        return Ok(result);
    }
}
