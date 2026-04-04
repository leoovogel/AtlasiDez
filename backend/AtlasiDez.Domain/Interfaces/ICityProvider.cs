using AtlasiDez.Domain.Entities;

namespace AtlasiDez.Domain.Interfaces;

public interface ICityProvider
{
    Task<IEnumerable<City>> GetByStateAsync(string uf);
}