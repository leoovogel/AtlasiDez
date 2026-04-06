namespace AtlasiDez.Infrastructure.Cache;

public class CacheOptions
{
    public const string SectionName = "Cache";

    public string RedisConnectionString { get; set; } = "localhost:6379";
    public int ExpirationInMinutes { get; set; } = 5;
}
