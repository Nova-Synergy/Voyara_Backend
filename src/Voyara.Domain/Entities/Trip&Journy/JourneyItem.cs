using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Entities.LocalInfo;
using Voyara.Domain.Enums;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.TripAndJourney;

/// <summary>
/// Represents a scheduled stop, visit, or activity within a single itinerary day of a journey.
/// </summary>
/// <remarks>
/// Links to a catalog <see cref="Destination"/>, an optional host <see cref="City"/>, and tracks visit sequence order,
/// scheduled time window, transportation mode, and visited status.
/// </remarks>
public sealed class JourneyItem : BaseEntity
{
    /// <summary>
    /// Gets the identifier of the parent journey day containing this item.
    /// </summary>
    /// <value>The foreign key pointing to the owning <see cref="JourneyDay"/>.</value>
    public Guid JourneyDayId { get; private set; }

    /// <summary>
    /// Gets the identifier of the catalog destination visited during this stop.
    /// </summary>
    /// <value>The foreign key pointing to the target <see cref="Destination"/>.</value>
    public Guid DestinationId { get; private set; }

    /// <summary>
    /// Gets the navigation reference to the visited destination.
    /// </summary>
    /// <value>The associated <see cref="Destination"/> entity.</value>
    public Destination? Destination { get; private set; }

    /// <summary>
    /// Gets the identifier of the city where this stop occurs, if applicable.
    /// </summary>
    /// <value>The optional integer foreign key pointing to the <see cref="City"/>.</value>
    public int? CityId { get; private set; }

    /// <summary>
    /// Gets the navigation reference to the city.
    /// </summary>
    /// <value>The associated <see cref="City"/> entity, or <see langword="null"/>.</value>
    public City? City { get; private set; }

    /// <summary>
    /// Gets the execution sequence order of this item within its parent day.
    /// </summary>
    /// <value>An integer indicating relative display and visit priority.</value>
    public int SortOrder { get; private set; }

    /// <summary>
    /// Gets the planned start time of day for this stop.
    /// </summary>
    /// <value>A <see cref="TimeSpan"/> offset from midnight, or <see langword="null"/> if unscheduled.</value>
    public TimeSpan? StartTime { get; private set; }

    /// <summary>
    /// Gets the planned end time of day for this stop.
    /// </summary>
    /// <value>A <see cref="TimeSpan"/> offset from midnight, or <see langword="null"/> if unscheduled.</value>
    public TimeSpan? EndTime { get; private set; }

    /// <summary>
    /// Gets the mode of transportation used to reach or explore this stop.
    /// </summary>
    /// <value>A <see cref="TransportMode"/> value.</value>
    public TransportMode Transport { get; private set; } = TransportMode.Walking;

    /// <summary>
    /// Gets a value indicating whether the tourist has visited or completed this stop.
    /// </summary>
    /// <value><see langword="true"/> if visited; otherwise, <see langword="false"/>.</value>
    public bool Visited { get; private set; }

    private JourneyItem()
    {
    }

    private JourneyItem(
        Guid journeyDayId,
        Guid destinationId,
        int? cityId,
        int sortOrder,
        TimeSpan? startTime,
        TimeSpan? endTime,
        TransportMode transport)
    {
        JourneyDayId = journeyDayId;
        DestinationId = destinationId;
        CityId = cityId;
        SortOrder = sortOrder;
        StartTime = startTime;
        EndTime = endTime;
        Transport = transport;
        Visited = false;
    }

    /// <summary>
    /// Factory method to construct a new <see cref="JourneyItem"/>.
    /// </summary>
    /// <param name="journeyDayId">The parent journey day identifier.</param>
    /// <param name="destinationId">The catalog destination identifier.</param>
    /// <param name="cityId">The optional city identifier.</param>
    /// <param name="sortOrder">Sequence order within the day.</param>
    /// <param name="startTime">Optional scheduled arrival or start time.</param>
    /// <param name="endTime">Optional scheduled departure or finish time.</param>
    /// <param name="transport">Mode of transportation to or during the visit.</param>
    /// <returns>A new <see cref="JourneyItem"/> instance.</returns>
    public static JourneyItem Create(
        Guid journeyDayId,
        Guid destinationId,
        int? cityId,
        int sortOrder,
        TimeSpan? startTime = null,
        TimeSpan? endTime = null,
        TransportMode transport = TransportMode.Walking)
    {
        return new JourneyItem(
            journeyDayId,
            destinationId,
            cityId,
            sortOrder,
            startTime,
            endTime,
            transport);
    }

    /// <summary>
    /// Marks the item as visited or unvisited.
    /// </summary>
    /// <param name="visited"><see langword="true"/> to mark completed; <see langword="false"/> to reset.</param>
    public void MarkVisited(bool visited = true)
    {
        Visited = visited;
    }

    /// <summary>
    /// Updates the scheduled time window for this visit.
    /// </summary>
    /// <param name="startTime">The revised start time.</param>
    /// <param name="endTime">The revised end time.</param>
    public void UpdateSchedule(TimeSpan? startTime, TimeSpan? endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }

    /// <summary>
    /// Updates the transportation mode for this item.
    /// </summary>
    /// <param name="transport">The revised <see cref="TransportMode"/>.</param>
    public void UpdateTransport(TransportMode transport)
    {
        Transport = transport;
    }

    /// <summary>
    /// Sets a new sequence position within the day.
    /// </summary>
    /// <param name="sortOrder">The revised sequence order index.</param>
    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
    }
}

