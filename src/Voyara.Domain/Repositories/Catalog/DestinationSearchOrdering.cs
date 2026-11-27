using System;
using System.Collections.Generic;
using System.Linq;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Enums;

namespace Voyara.Domain.Repositories.Catalog;

/// <summary>
/// The ordering rules of destination search. Database-translatable ordering is used for numeric/date fields;
/// localized name ordering is performed in memory with ordinal comparison to avoid database collation differences.
/// </summary>
public static class DestinationSearchOrdering
{
    /// <summary>
    /// Orders database results by numeric/date keys. The direction applies to every key of the chosen sort (for rating,
    /// to the average rating and to the review count). Use <see cref="ApplyInMemory"/> for name sorting so collation
    /// does not vary by provider. The destination id is the final ascending tie-breaker.
    /// </summary>
    public static IOrderedQueryable<Destination> Apply(IQueryable<Destination> query, DestinationSearchCriteria criteria)
    {
        if (criteria.SortBy == DestinationSortBy.Name)
        {
            throw new InvalidOperationException(
                "Name sorting must use ApplyInMemory so the result does not depend on database collation.");
        }

        var descending = criteria.EffectiveSortDirection == SortDirection.Descending;

        IOrderedQueryable<Destination> ordered = criteria.SortBy switch
        {
            DestinationSortBy.Rating => descending
                ? query.OrderByDescending(destination => destination.AverageRating)
                    .ThenByDescending(destination => destination.ReviewCount)
                : query.OrderBy(destination => destination.AverageRating)
                    .ThenBy(destination => destination.ReviewCount),
            DestinationSortBy.Newest => descending
                ? query.OrderByDescending(destination => destination.CreatedAt)
                : query.OrderBy(destination => destination.CreatedAt),
            _ => descending
                ? query.OrderByDescending(destination => destination.PopularityScore)
                : query.OrderBy(destination => destination.PopularityScore)
        };

        return ordered.ThenBy(destination => destination.Id);
    }

    /// <summary>
    /// Applies deterministic application-level ordering to a materialized result set.
    /// Name sorting uses ordinal comparison here so it does not depend on the database collation.
    /// </summary>
    /// <param name="destinations">The filtered destinations with translations loaded.</param>
    /// <param name="criteria">The requested sort criteria.</param>
    /// <returns>The ordered destinations, with identifier as a stable final tie-breaker.</returns>
    public static IOrderedEnumerable<Destination> ApplyInMemory(
        IEnumerable<Destination> destinations,
        DestinationSearchCriteria criteria)
    {
        var descending = criteria.EffectiveSortDirection == SortDirection.Descending;

        IOrderedEnumerable<Destination> ordered = criteria.SortBy switch
        {
            DestinationSortBy.Rating => descending
                ? destinations.OrderByDescending(destination => destination.AverageRating)
                    .ThenByDescending(destination => destination.ReviewCount)
                : destinations.OrderBy(destination => destination.AverageRating)
                    .ThenBy(destination => destination.ReviewCount),
            DestinationSortBy.Name => descending
                ? destinations.OrderByDescending(destination => GetDisplayName(destination, criteria.LanguageCode), StringComparer.Ordinal)
                : destinations.OrderBy(destination => GetDisplayName(destination, criteria.LanguageCode), StringComparer.Ordinal),
            DestinationSortBy.Newest => descending
                ? destinations.OrderByDescending(destination => destination.CreatedAt)
                : destinations.OrderBy(destination => destination.CreatedAt),
            _ => descending
                ? destinations.OrderByDescending(destination => destination.PopularityScore)
                : destinations.OrderBy(destination => destination.PopularityScore)
        };

        return ordered.ThenBy(destination => destination.Id);
    }

    /// <summary>Gets the same localized display name selected by the response mapper.</summary>
    private static string? GetDisplayName(Destination destination, string languageCode)
    {
        var requested = destination.Translations.FirstOrDefault(translation =>
            string.Equals(translation.LanguageCode, languageCode, StringComparison.OrdinalIgnoreCase));
        if (requested is not null)
        {
            return requested.Name;
        }

        var english = destination.Translations.FirstOrDefault(translation =>
            string.Equals(translation.LanguageCode, DestinationSearchCriteria.DefaultLanguageCode, StringComparison.OrdinalIgnoreCase));
        if (english is not null)
        {
            return english.Name;
        }

        return destination.Translations
            .OrderBy(translation => translation.LanguageCode, StringComparer.Ordinal)
            .Select(translation => translation.Name)
            .FirstOrDefault();
    }

}
