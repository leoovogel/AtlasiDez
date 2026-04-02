using AtlasiDez.Application.UseCases;

namespace AtlasiDez.Api.Endpoints;

public static class CityEndpoints
{
    public static void MapCityEndpoints(this WebApplication app)
    {
        app.MapGet("/api/cities/{uf}", async (
            string uf,
            GetCitiesByStateUseCase useCase,
            int page = 1,
            int pageSize = 10) =>
        {
            var result = await useCase.ExecuteAsync(uf, page, pageSize);
            return Results.Ok(result);
        });
    }
}