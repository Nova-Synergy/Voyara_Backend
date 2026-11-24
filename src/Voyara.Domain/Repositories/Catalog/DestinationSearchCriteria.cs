using Voyara.Domain.Enums;

namespace Voyara.Domain.Repositories.Catalog;

/// <summary>
/// Search specification for the public destination catalog. Each search feature adds its own optional
/// members, so the repository contract does not change when a new filter is introduced.
/// </summary>
public sealed record DestinationSearchCriteria
{
    /// <summary>
    /// Free text matched, case-insensitively and as a literal substring, against the name and description of
    /// every translation of a destination (the match is language-independent, so a visitor can search in any
    /// language). Null or empty means no text filter.
    /// </summary>
    public string? Text { get; init; }

    /// <summary>Restricts results to destinations of cities in this country; null means any country.</summary>
    public int? CountryId { get; init; }

    /// <summary>Restricts results to destinations in this city; null means any city.</summary>
    public int? CityId { get; init; }

    /// <summary>Restricts results to destinations of this category; null means any category.</summary>
    public Guid? CategoryId { get; init; }

    /// <summary>Minimum average rating (0 to 5, inclusive); null means no rating filter.</summary>
    public double? MinRating { get; init; }

    /// <summary>
    /// Maximum ticket price, in <see cref="Currency"/>. A destination matches when at least one of its ticket
    /// prices in that currency is at or below the limit; destinations without a price in that currency do not match.
    /// Null means no price filter.
    /// </summary>
    public decimal? MaxPrice { get; init; }

    /// <summary>ISO 4217 currency code (upper-case) that <see cref="MaxPrice"/> is expressed in.</summary>
    public string? Currency { get; init; }

    /// <summary>English: the default language and the first fallback when a translation is missing.</summary>
    public const string DefaultLanguageCode = "en";

    /// <summary>
    /// Language whose translation provides the name used by <see cref="DestinationSortBy.Name"/> sorting (falling back
    /// to English, then to the lowest language code, exactly like the displayed name). It does not affect text matching,
    /// which looks at every translation.
    /// </summary>
    public string LanguageCode { get; init; } = DefaultLanguageCode;

    /// <summary>Sort key; popularity by default.</summary>
    public DestinationSortBy SortBy { get; init; } = DestinationSortBy.Popularity;

    /// <summary>
    /// Sort direction; null uses the natural direction of <see cref="SortBy"/>
    /// (descending for popularity, rating and newest; ascending for name). The direction applies to every key of the
    /// chosen sort (for rating, to the average rating and to the review count); the destination id is always the
    /// final ascending tie-breaker.
    /// </summary>
    public SortDirection? SortDirection { get; init; }

    /// <summary>The direction actually applied: the requested one, or the natural direction of <see cref="SortBy"/>.</summary>
    public SortDirection EffectiveSortDirection => SortDirection ?? SortBy.DefaultDirection();
}
