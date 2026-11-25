namespace Voyara.Application.Identity.Common;

/// <summary>Accepted formats and limits for tourist profile images.</summary>
internal static class ProfileImageRules
{
    /// <summary>Largest accepted image size in bytes (5 MB).</summary>
    public const long MaxBytes = 5 * 1024 * 1024;

    private static readonly Dictionary<string, string> ExtensionByContentType = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/jpeg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/webp"] = ".webp"
    };

    private static readonly Dictionary<string, string> ContentTypeByExtension = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = "image/jpeg",
        [".png"] = "image/png",
        [".webp"] = "image/webp"
    };

    /// <summary>Returns whether the declared content type is an accepted image type.</summary>
    public static bool IsAllowedContentType(string? contentType)
        => contentType is not null && ExtensionByContentType.ContainsKey(contentType.Trim());

    /// <summary>
    /// Detects the real image format from the file signature (JPEG, PNG or WebP) and returns
    /// its content type and extension; null when the bytes are not an accepted image.
    /// </summary>
    public static (string ContentType, string Extension)? Detect(ReadOnlySpan<byte> header)
    {
        if (header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
        {
            return ("image/jpeg", ".jpg");
        }

        ReadOnlySpan<byte> png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        if (header.Length >= png.Length && header[..png.Length].SequenceEqual(png))
        {
            return ("image/png", ".png");
        }

        ReadOnlySpan<byte> riff = "RIFF"u8;
        ReadOnlySpan<byte> webp = "WEBP"u8;
        if (header.Length >= 12 && header[..4].SequenceEqual(riff) && header.Slice(8, 4).SequenceEqual(webp))
        {
            return ("image/webp", ".webp");
        }

        return null;
    }

    /// <summary>Resolves the content type of a stored file from its extension; null when unknown.</summary>
    public static string? ContentTypeFromFileName(string fileName)
        => ContentTypeByExtension.GetValueOrDefault(Path.GetExtension(fileName));

    /// <summary>
    /// Returns whether the stored photo value refers to a file held by <c>IFileStorage</c>
    /// (a plain file key) rather than an external absolute URL.
    /// </summary>
    public static bool IsStoredFile(string? photoUrl)
        => !string.IsNullOrWhiteSpace(photoUrl)
           && !Uri.TryCreate(photoUrl, UriKind.Absolute, out _)
           && photoUrl == Path.GetFileName(photoUrl);
}
