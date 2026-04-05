using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using AtlasiDez.Domain.Entities;
using AtlasiDez.Domain.Interfaces;

namespace AtlasiDez.Infrastructure.Providers;

[ExcludeFromCodeCoverage]
public class IbgeCityProvider(HttpClient httpClient) : ICityProvider
{
    public async Task<IEnumerable<City>> GetByStateAsync(string uf)
    {
        var response = await httpClient.GetFromJsonAsync<List<IbgeCity>>(
            $"https://servicodados.ibge.gov.br/api/v1/localidades/estados/{uf}/municipios");

        return response?.Select(c => new City(c.Nome, c.Id.ToString())) ?? [];
    }

    private record IbgeCity(
        [property: JsonPropertyName("nome")] string Nome,
        [property: JsonPropertyName("id")] long Id
    );
}