namespace Voyara.Domain.Enums;

/// <summary>
/// Represents the execution and planning state of an itinerary journey.
/// </summary>
/// <remarks>
/// Tracks how far a journey plan has progressed from initial generation through active execution.
/// </remarks>
public enum JourneyStatus
{
    /// <summary>
    /// Initial drafting state where days and destinations are being arranged or generated.
    /// </summary>
    Draft,

    /// <summary>
    /// The itinerary has been fully structured, validated, and confirmed.
    /// </summary>
    Planned,

    /// <summary>
    /// The journey is currently being traveled according to its scheduled days.
    /// </summary>
    InProgress,

    /// <summary>
    /// All planned days and activities in the journey have completed.
    /// </summary>
    Completed,

    /// <summary>
    /// The journey was cancelled prior to or during travel.
    /// </summary>
    Cancelled
}

