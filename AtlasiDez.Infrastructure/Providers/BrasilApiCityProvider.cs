using System.Net.Http.Json;
using System.Text.Json.Serialization;
using AtlasiDez.Domain.Entities;
using AtlasiDez.Domain.Interfaces;

namespace AtlasiDez.Infrastructure.Providers;

public class BrasilApiCityProvider(HttpClient httpClient) : ICityProvider
{
    public async Task<IEnumerable<City>> GetByStateAsync(string uf)
    {
        var response = await httpClient.GetFromJsonAsync<List<BrasilApiCity>>(
            $"https://brasilapi.com.br/api/ibge/municipios/v1/{uf}");

        return response?.Select(c => new City(c.Nome, c.CodigoIbge)) ?? [];
    }

    private record BrasilApiCity(
        [property: JsonPropertyName("nome")] string Nome,
        [property: JsonPropertyName("codigo_ibge")] string CodigoIbge
    );
}