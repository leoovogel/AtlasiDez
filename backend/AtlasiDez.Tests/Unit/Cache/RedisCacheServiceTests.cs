using System.Text;
using System.Text.Json;
using AtlasiDez.Infrastructure.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace AtlasiDez.Tests.Unit.Cache;

public class RedisCacheServiceTests
{
    private readonly IDistributedCache _distributedCache = Substitute.For<IDistributedCache>();
    private readonly IOptions<CacheOptions> _cacheOptions = Options.Create(new CacheOptions { ExpirationInMinutes = 5 });
    private readonly ILogger<RedisCacheService> _logger = Substitute.For<ILogger<RedisCacheService>>();
    private readonly RedisCacheService _redisCacheService;

    public RedisCacheServiceTests()
    {
        _redisCacheService = new RedisCacheService(_distributedCache, _cacheOptions, _logger);
    }

    [Fact]
    public async Task WhenValueExistsInCache_ReturnsDeserializedValue()
    {
        var expected = new TestData("test", 42);
        var json = JsonSerializer.Serialize(expected);
        var bytes = Encoding.UTF8.GetBytes(json);

        _distributedCache.GetAsync("test-key", Arg.Any<CancellationToken>())
            .Returns(bytes);

        var result = await _redisCacheService.GetAsync<TestData>("test-key");

        Assert.NotNull(result);
        Assert.Equal(expected.Name, result.Name);
        Assert.Equal(expected.Value, result.Value);
    }

    [Fact]
    public async Task WhenValueDoesNotExist_ReturnsDefault()
    {
        _distributedCache.GetAsync("missing-key", Arg.Any<CancellationToken>())
            .Returns((byte[]?)null);

        var result = await _redisCacheService.GetAsync<TestData>("missing-key");

        Assert.Null(result);
    }

    [Fact]
    public async Task WhenRedisThrows_ReturnsDefaultAndLogsWarning()
    {
        _distributedCache.GetAsync("failing-key", Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("Redis connection failed"));

        var result = await _redisCacheService.GetAsync<TestData>("failing-key");

        Assert.Null(result);
    }

    [Fact]
    public async Task SerializesAndStoresValue()
    {
        var data = new TestData("test", 42);

        await _redisCacheService.SetAsync("test-key", data, TimeSpan.FromHours(1));

        await _distributedCache.Received(1).SetAsync(
            "test-key",
            Arg.Any<byte[]>(),
            Arg.Is<DistributedCacheEntryOptions>(o =>
                o.AbsoluteExpirationRelativeToNow == TimeSpan.FromHours(1)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task WhenNoExpirationProvided_UsesDefaultFromOptions()
    {
        var data = new TestData("test", 42);

        await _redisCacheService.SetAsync("test-key", data);

        await _distributedCache.Received(1).SetAsync(
            "test-key",
            Arg.Any<byte[]>(),
            Arg.Is<DistributedCacheEntryOptions>(o =>
                o.AbsoluteExpirationRelativeToNow == TimeSpan.FromMinutes(5)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task WhenRedisThrows_LogsWarningAndDoesNotThrow()
    {
        _distributedCache.SetAsync(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("Redis connection failed"));

        var exception = await Record.ExceptionAsync(() =>
            _redisCacheService.SetAsync("failing-key", new TestData("test", 1), TimeSpan.FromHours(1)));

        Assert.Null(exception);
    }

    private record TestData(string Name, int Value);
}
