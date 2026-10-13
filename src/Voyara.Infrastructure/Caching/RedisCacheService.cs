using System.Text.Json;
using StackExchange.Redis;
using Voyara.Application.Abstractions;

namespace Voyara.Infrastructure.Caching;

/// <summary>Redis-backed cache with tag sets for group invalidation.</summary>
public sealed class RedisCacheService : ICacheService
{
    private const string TagPrefix = "voyara:tag:";

    private readonly IConnectionMultiplexer _multiplexer;

    /// <summary>Creates the cache service.</summary>
    public RedisCacheService(IConnectionMultiplexer multiplexer) => _multiplexer = multiplexer;

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var database = _multiplexer.GetDatabase();
        var value = await database.StringGetAsync(key);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : default;
    }

    /// <inheritdoc />
    public async Task SetAsync(
        string key,
        object value,
        TimeSpan? expiration = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default)
    {
        var database = _multiplexer.GetDatabase();
        await database.StringSetAsync(key, JsonSerializer.Serialize(value), expiration);

        if (tags is not null)
        {
            foreach (var tag in tags)
            {
                await database.SetAddAsync(TagPrefix + tag, key);
            }
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var database = _multiplexer.GetDatabase();
        await database.KeyDeleteAsync(key);
    }

    /// <inheritdoc />
    public async Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        var database = _multiplexer.GetDatabase();
        var tagKey = TagPrefix + tag;
        var members = await database.SetMembersAsync(tagKey);

        if (members.Length > 0)
        {
            var keys = members.Select(m => (RedisKey)(string) m!).ToArray();
            await database.KeyDeleteAsync(keys);
        }

        await database.KeyDeleteAsync(tagKey);
    }
}
