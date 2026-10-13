namespace Voyara.Domain.Enums;

/// <summary>Lifecycle status of a destination.</summary>
public enum DestinationStatus
{
    /// <summary>Created but not publicly visible.</summary>
    Draft = 1,

    /// <summary>Publicly visible and searchable.</summary>
    Published = 2,

    /// <summary>Retired from the public catalog but kept for history.</summary>
    Archived = 3
}
