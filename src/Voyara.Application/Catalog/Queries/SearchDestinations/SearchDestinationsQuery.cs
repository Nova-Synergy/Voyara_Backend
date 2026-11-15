using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Catalog;

namespace Voyara.Application.Catalog.Queries.SearchDestinations;

/// <summary>
/// Searches the public destination catalog by free text with paging; cached under a stable key and
/// invalidated by tag.
/// </summary>
/// <param name="SearchTerm">Text matched against destination names and descriptions; null returns everything.</param>
/// <param name="LanguageCode">Language of the returned texts; null means English.</param>
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
    int? CountryId,
    int? CityId,
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
    /// <summary>Normalized language code (lower-case); English when none was requested.</summary>
    public string Language => string.IsNullOrWhiteSpace(LanguageCode)
        ? DestinationSearchCriteria.DefaultLanguageCode
        : LanguageCode.Trim().ToLowerInvariant();

    /// <summary>Normalized currency code (upper-case); null when none was supplied.</summary>
    public string? NormalizedCurrency => string.IsNullOrWhiteSpace(Currency) ? null : Currency.Trim().ToUpperInvariant();

    /// <inheritdoc />
    public string CacheKey =>
        $"destinations:search:{Language}:{Uri.EscapeDataString(SearchTerm?.Trim().ToLowerInvariant() ?? string.Empty)}"
        + $":country={CountryId}:city={CityId}:category={CategoryId}"
        + $":rating={MinRating}:price={MaxPrice}{NormalizedCurrency}"
        + $":sort={SortBy}{SortDirection}:{Page}:{PageSize}";

    /// <inheritdoc />
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);

    /// <inheritdoc />
    public IReadOnlyCollection<string> Tags => new[] { "destinations" };
}
