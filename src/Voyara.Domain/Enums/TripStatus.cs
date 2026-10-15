namespace Voyara.Domain.Enums;

/// <summary>
/// Specifies the lifecycle stages of a planned tourist trip.
/// </summary>
/// <remarks>
/// Status transitions typically move sequentially from <see cref="Draft"/> to <see cref="Upcoming"/>,
/// <see cref="Ongoing"/>, and <see cref="Completed"/>, but a trip can be transitioned to <see cref="Cancelled"/>
/// at any point prior to completion.
/// </remarks>
public enum TripStatus
{
    /// <summary>
    /// Indicates that the trip is being drafted or customized and has not yet been finalized.
    /// </summary>
    Draft,

    /// <summary>
    /// Indicates that the trip itinerary is finalized and scheduled for a future date.
    /// </summary>
    Upcoming,

    /// <summary>
    /// Indicates that the trip is currently underway.
    /// </summary>
    Ongoing,

    /// <summary>
    /// Indicates that the trip has concluded.
    /// </summary>
    Completed,

    /// <summary>
    /// Indicates that the trip was aborted before or during execution.
    /// </summary>
    Cancelled
}

