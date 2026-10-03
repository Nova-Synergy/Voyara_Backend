namespace Voyara.Infrastructure.Persistence;

/// <summary>PostgreSQL connection settings.</summary>
public sealed class DatabaseOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Database";

    /// <summary>Npgsql connection string; provided via configuration, never hard-coded.</summary>
    public string ConnectionString { get; set; } = string.Empty;
}
