using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;
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
/// <param name="Page">One-based page number.</param>
/// <param name="PageSize">Items per page.</param>
public sealed record SearchDestinationsQuery(
    string? SearchTerm,
    string? LanguageCode,
    int? CountryId,
    int? CityId,
    Guid? CategoryId,
    int Page,
    int PageSize)
    : ICachedQuery<PagedResult<DestinationSummaryResponse>>
{
    /// <summary>Normalized language code (lower-case); English when none was requested.</summary>
    public string Language => string.IsNullOrWhiteSpace(LanguageCode)
        ? DestinationSearchCriteria.DefaultLanguageCode
        : LanguageCode.Trim().ToLowerInvariant();

    /// <inheritdoc />
    public string CacheKey =>
        $"destinations:search:{Language}:{Uri.EscapeDataString(SearchTerm?.Trim().ToLowerInvariant() ?? string.Empty)}"
        + $":country={CountryId}:city={CityId}:category={CategoryId}:{Page}:{PageSize}";

    /// <inheritdoc />
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);

    /// <inheritdoc />
    public IReadOnlyCollection<string> Tags => new[] { "destinations" };
}
