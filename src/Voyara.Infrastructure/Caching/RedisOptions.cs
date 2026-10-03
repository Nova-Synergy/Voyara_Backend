namespace Voyara.Infrastructure.Caching;

/// <summary>Redis connection settings.</summary>
public sealed class RedisOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Redis";

    /// <summary>StackExchange.Redis connection string.</summary>
    public string ConnectionString { get; set; } = "localhost:6379";
}
