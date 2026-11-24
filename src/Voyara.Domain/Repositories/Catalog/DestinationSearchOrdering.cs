using System.Linq.Expressions;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Enums;

namespace Voyara.Domain.Repositories.Catalog;

/// <summary>
/// The ordering rules of destination search, written as plain LINQ so that the persistence layer translates them
/// to SQL and unit tests can run them in memory.
/// </summary>
public static class DestinationSearchOrdering
{
    /// <summary>
    /// Orders the results by the requested key. The direction applies to every key of the chosen sort (for rating, to
    /// the average rating and to the review count). The destination id is always the final ascending tie-breaker, so
    /// the order is deterministic and consecutive pages never repeat or skip an item.
    /// </summary>
    public static IOrderedQueryable<Destination> Apply(IQueryable<Destination> query, DestinationSearchCriteria criteria)
    {
        var descending = criteria.EffectiveSortDirection == SortDirection.Descending;

        IOrderedQueryable<Destination> ordered = criteria.SortBy switch
        {
            DestinationSortBy.Rating => descending
                ? query.OrderByDescending(destination => destination.AverageRating)
                    .ThenByDescending(destination => destination.ReviewCount)
                : query.OrderBy(destination => destination.AverageRating)
                    .ThenBy(destination => destination.ReviewCount),
            DestinationSortBy.Name => descending
                ? query.OrderByDescending(DisplayName(criteria.LanguageCode))
                : query.OrderBy(DisplayName(criteria.LanguageCode)),
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
    /// The name shown for the requested language: the translation in that language, else English, else the one with
    /// the lowest language code (the same rule as the response mapping). It is a single correlated subquery that ranks
    /// the destination's translations; a destination has at most one translation per language code (composite key), so
    /// the chosen translation is always unique.
    /// </summary>
    public static Expression<Func<Destination, string?>> DisplayName(string languageCode)
        => destination => destination.Translations
            .OrderBy(translation => translation.LanguageCode == languageCode
                ? 0
                : translation.LanguageCode == DestinationSearchCriteria.DefaultLanguageCode ? 1 : 2)
            .ThenBy(translation => translation.LanguageCode)
            .Select(translation => translation.Name)
            .FirstOrDefault();
}
