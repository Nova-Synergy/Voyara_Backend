namespace Voyara.Domain.Enums;

/// <summary>
/// Represents the publication lifecycle status of a destination in the catalog.
/// </summary>
public enum DestinationStatus
{
    /// <summary>
    /// Destination is in draft state and not visible to public searches.
    /// </summary>
    Draft,

    /// <summary>
    /// Destination is published and visible to tourists.
    /// </summary>
    Published,

    /// <summary>
    /// Destination is archived and read-only.
    /// </summary>
    Archived
}

