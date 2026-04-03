using System.Text.Json;
using AtlasiDez.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace AtlasiDez.Infrastructure.Cache;

public class RedisCacheService(
    IDistributedCache distributedCache,
    IOptions<CacheOptions> cacheOptions) : ICacheService
{
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromHours(cacheOptions.Value.ExpirationInHours);

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var json = await distributedCache.GetStringAsync(key);

            return json is null
                ? default
                : JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Falha ao obter valor do cache Redis para a chave '{key}': {ex.Message}");
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration
            };

            await distributedCache.SetStringAsync(key, json, options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Falha ao definir valor no cache Redis para a chave '{key}': {ex.Message}");
        }
    }
}
