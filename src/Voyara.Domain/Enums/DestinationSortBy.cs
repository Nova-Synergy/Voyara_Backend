namespace Voyara.Domain.Enums;

/// <summary>Sort keys supported by destination search.</summary>
public enum DestinationSortBy
{
    /// <summary>Most popular first by default.</summary>
    Popularity,

    /// <summary>Best rated first by default; ties are broken by the number of reviews.</summary>
    Rating,

    /// <summary>Alphabetical by the displayed (translated) name, A to Z by default.</summary>
    Name,

    /// <summary>Most recently created first by default.</summary>
    Newest
}
