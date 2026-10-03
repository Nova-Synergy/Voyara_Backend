namespace Voyara.Application.Abstractions.Messaging;

/// <summary>A query whose result is cached under <see cref="CacheKey"/> and invalidated by tags.</summary>
public interface ICachedQuery<TResponse> : IQuery<TResponse>
{
    /// <summary>Cache key uniquely identifying the query result.</summary>
    string CacheKey { get; }

    /// <summary>Optional sliding lifetime; null uses the cache default.</summary>
    TimeSpan? Expiration { get; }

    /// <summary>Tags used for group invalidation when related data changes.</summary>
    IReadOnlyCollection<string> Tags { get; }
}
