using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;
using Voyara.Domain.Enums;

namespace Voyara.Application.Catalog.Queries.SearchDestinations;

/// <summary>
/// Searches the public destination catalog by free text with paging; cached under a normalized key and
/// invalidated by the <see cref="CatalogCacheTags.Destinations"/> tag.
/// </summary>
/// <param name="SearchTerm">Text matched against destination names and descriptions; null or blank returns everything.</param>
/// <param name="LanguageCode">Language of the returned texts; null means English. Must be a supported language.</param>
/// <param name="CountryId">Only destinations of cities in this country; null means any country.</param>
/// <param name="CityId">Only destinations in this city; null means any city.</param>
/// <param name="CategoryId">Only destinations of this category; null means any category.</param>
/// <param name="MinRating">Minimum average rating (0 to 5); null means no rating filter.</param>
/// <param name="MaxPrice">Maximum ticket price in <paramref name="Currency"/>; null means no price filter.</param>
/// <param name="Currency">ISO 4217 code of <paramref name="MaxPrice"/>; required when a price limit is given.</param>
/// <param name="SortBy">Sort key.</param>
/// <param name="SortDirection">Sort direction; null uses the natural direction of the sort key.</param>
/// <param name="Page">One-based page number.</param>
/// <param name="PageSize">Items per page (1 to 100).</param>
public sealed record SearchDestinationsQuery(
    string? SearchTerm,
    string? LanguageCode,
    Guid? CountryId,
    Guid? CityId,
    Guid? CategoryId,
    double? MinRating,
    decimal? MaxPrice,
    string? Currency,
    DestinationSortBy SortBy,
    SortDirection? SortDirection,
    int Page,
    int PageSize)
    : ICachedQuery<PagedResult<DestinationSummaryResponse>>
{
    /// <summary>Language used when none is requested.</summary>
    public const string DefaultLanguage = "en";

    /// <summary>Normalized language code (trimmed, lower-case); English when none was requested.</summary>
    public string Language => string.IsNullOrWhiteSpace(LanguageCode)
        ? DefaultLanguage
        : LanguageCode.Trim().ToLowerInvariant();

    /// <summary>
    /// The search term with surrounding and repeated whitespace removed; null when it is null, empty or blank.
    /// Used for both the repository filter and the cache key, so null, "" and "   " are the same search.
    /// </summary>
    public string? NormalizedSearchTerm
    {
        get
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                return null;
            }

            return string.Join(' ', SearchTerm.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        }
    }

    /// <summary>The direction actually applied: the requested one, or the natural direction of the sort key.</summary>
    public SortDirection EffectiveSortDirection => SortDirection ?? SortBy.DefaultDirection();

    /// <summary>Normalized currency code (trimmed, upper-case); null when none was supplied.</summary>
    public string? NormalizedCurrency => string.IsNullOrWhiteSpace(Currency) ? null : Currency.Trim().ToUpperInvariant();

    /// <inheritdoc />
    public string CacheKey
    {
        get
        {
            // Numbers are formatted with the invariant culture and in a canonical form (4 and 4.0, or 100.0 and
            // 100.00, are the same filter), so the key never depends on the server culture or on the input scale.
            var filters = FormattableString.Invariant(
                $"country={CountryId}|city={CityId}|category={CategoryId}|rating={FormatRating(MinRating)}|price={FormatPrice(MaxPrice)}|currency={NormalizedCurrency}");

            return FormattableString.Invariant(
                $"destinations:search:v1|lang={Language}|q={TermFingerprint(NormalizedSearchTerm)}|{filters}|sort={SortBy}:{EffectiveSortDirection}|page={Page}|size={PageSize}");
        }
    }

    /// <inheritdoc />
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);

    /// <inheritdoc />
    public IReadOnlyCollection<string> Tags => new[] { CatalogCacheTags.Destinations };

    private static string FormatRating(double? rating)
        => rating?.ToString("R", CultureInfo.InvariantCulture) ?? string.Empty;

    // Dividing by 1.000...0 removes trailing zeros of the decimal's scale without changing its value.
    private static string FormatPrice(decimal? price)
        => price is null
            ? string.Empty
            : (price.Value / 1.0000000000000000000000000000m).ToString(CultureInfo.InvariantCulture);

    /// <summary>
    /// Fixed-length, case-insensitive fingerprint of the term, so arbitrary user input never becomes part of a
    /// cache key (bounded key length, no separator or injection characters).
    /// </summary>
    private static string TermFingerprint(string? normalizedTerm)
    {
        if (normalizedTerm is null)
        {
            return "-";
        }

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(normalizedTerm.ToLowerInvariant()));
        return Convert.ToHexString(hash, 0, 16).ToLowerInvariant();
    }
}
