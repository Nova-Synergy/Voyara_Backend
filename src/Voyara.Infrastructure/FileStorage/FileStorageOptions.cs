namespace Voyara.Infrastructure.FileStorage;

/// <summary>Local file storage settings.</summary>
public sealed class FileStorageOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "FileStorage";

    /// <summary>Root directory for stored files; created when missing.</summary>
    public string RootPath { get; set; } = "file-storage";
}
