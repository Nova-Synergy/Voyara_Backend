namespace Voyara.Application.Abstractions;

/// <summary>Cache abstraction with tag-based invalidation only.</summary>
public interface ICacheService
{
    /// <summary>Reads a cached value; null on miss.</summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>Stores a value under the key with an optional lifetime and cache tags.</summary>
    Task SetAsync(
        string key,
        object value,
        TimeSpan? expiration = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default);

    /// <summary>Removes a single cached value.</summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>Invalidates every cached value registered under the tag.</summary>
    Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default);
}
