namespace Voyara.Domain.Repositories.Catalog;

/// <summary>
/// Search specification for the public destination catalog. Each search feature adds its own optional
/// members, so the repository contract does not change when a new filter is introduced.
/// </summary>
public sealed record DestinationSearchCriteria
{
    /// <summary>Default language used to pick the display translation of a destination.</summary>
    public const string DefaultLanguageCode = "en";

    /// <summary>Free text matched against the name and description of every translation; null means no text filter.</summary>
    public string? Text { get; init; }

    /// <summary>Language used to choose the display translation (falls back to English, then to any translation).</summary>
    public string LanguageCode { get; init; } = DefaultLanguageCode;
}
