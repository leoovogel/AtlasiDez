using AtlasiDez.Domain.Entities;
using AtlasiDez.Domain.Interfaces;

namespace AtlasiDez.Tests.Integration.Fakes;

public class FakeCityProvider : ICityProvider
{
    private readonly Dictionary<string, List<City>> _data = new(StringComparer.OrdinalIgnoreCase);
    private Exception? _exception;

    public void SetCities(string uf, List<City> cities)
    {
        _data[uf] = cities;
    }

    public void SetException(Exception exception)
    {
        _exception = exception;
    }

    public void Reset()
    {
        _data.Clear();
        _exception = null;
    }

    public Task<IEnumerable<City>> GetByStateAsync(string uf)
    {
        if (_exception is not null)
            throw _exception;

        if (_data.TryGetValue(uf, out var cities))
            return Task.FromResult<IEnumerable<City>>(cities);

        return Task.FromResult<IEnumerable<City>>([]);
    }
}
