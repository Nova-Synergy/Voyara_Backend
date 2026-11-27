namespace Voyara.Domain.Enums;

/// <summary>Sort keys supported by destination search.</summary>
public enum DestinationSortBy
{
    /// <summary>Most popular first by default.</summary>
    Popularity,

    /// <summary>
    /// Average rating, then number of reviews, both in the requested direction (best rated and most reviewed first
    /// by default).
    /// </summary>
    Rating,

    /// <summary>Alphabetical by the displayed (translated) name, A to Z by default.</summary>
    Name,

    /// <summary>Most recently created first by default.</summary>
    Newest
}

/// <summary>Helpers for <see cref="DestinationSortBy"/>.</summary>
public static class DestinationSortByExtensions
{
    /// <summary>
    /// The natural direction of a sort key: ascending for the name (A to Z), descending for popularity, rating and
    /// recency (most popular, best rated and newest first).
    /// </summary>
    public static SortDirection DefaultDirection(this DestinationSortBy sortBy)
        => sortBy == DestinationSortBy.Name ? SortDirection.Ascending : SortDirection.Descending;
}
