namespace Voyara.Infrastructure.Persistence;

/// <summary>SQL Server connection settings.</summary>
public sealed class DatabaseOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Database";

    /// <summary>SQL Server connection string; provided via configuration, never hard-coded.</summary>
    public string ConnectionString { get; set; } = string.Empty;
}
