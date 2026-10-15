namespace Voyara.Domain.Enums;

/// <summary>
/// Defines access control and discoverability levels for trips and itineraries.
/// </summary>
/// <remarks>
/// Governs whether a trip is restricted to the owning tourist, shared via tokenized links,
/// or publicly visible in community discovery feeds.
/// </remarks>
public enum Visibility
{
    /// <summary>
    /// The trip is restricted exclusively to the account owner and platform administrators.
    /// </summary>
    Private,

    /// <summary>
    /// The trip is accessible to collaborators or viewers holding a valid share token.
    /// </summary>
    Shared,

    /// <summary>
    /// The trip is publicly searchable and viewable across the platform community.
    /// </summary>
    Public
}

