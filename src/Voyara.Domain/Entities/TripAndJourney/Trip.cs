using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Enums;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.TripAndJourney;

/// <summary>
/// Aggregate root representing an overarching tourist trip booking and schedule.
/// </summary>
/// <remarks>
/// Encapsulates trip calendar dates, owning user identity, visibility permissions,
/// sharing tokens (<see cref="TripShare"/>), and the operational itinerary (<see cref="Journey"/>).
/// Enforces business invariants such as start date occurring on or before end date.
/// </remarks>
public sealed class Trip : AggregateRoot
{
    private readonly List<TripShare> _shares = new();

    /// <summary>
    /// Gets the identifier of the tourist user who owns this trip.
    /// </summary>
    /// <value>The foreign key linking to the owning <see cref="User"/>.</value>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the navigation reference to the owning user.
    /// </summary>
    /// <value>The <see cref="User"/> entity.</value>
    public User? User { get; private set; }

    /// <summary>
    /// Gets the user-defined title of the trip.
    /// </summary>
    /// <value>A descriptive title string.</value>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the planned departure/start date of the trip.
    /// </summary>
    /// <value>The start <see cref="DateOnly"/>.</value>
    public DateOnly StartDate { get; private set; }

    /// <summary>
    /// Gets the planned return/end date of the trip.
    /// </summary>
    /// <value>The end <see cref="DateOnly"/>.</value>
    public DateOnly EndDate { get; private set; }

    /// <summary>
    /// Gets the current lifecycle status of the trip.
    /// </summary>
    /// <value>A <see cref="TripStatus"/> value denoting progression.</value>
    public TripStatus Status { get; private set; } = TripStatus.Draft;

    /// <summary>
    /// Gets the privacy and sharing scope of the trip.
    /// </summary>
    /// <value>A <see cref="Visibility"/> value defining access permissions.</value>
    public Visibility Visibility { get; private set; } = Visibility.Private;

    /// <summary>
    /// Gets the identifier of the linked journey itinerary, if created.
    /// </summary>
    /// <value>The optional foreign key pointing to the associated <see cref="Journey"/>.</value>
    public Guid? JourneyId { get; private set; }

    /// <summary>
    /// Gets the navigation reference to the linked journey itinerary.
    /// </summary>
    /// <value>The associated <see cref="Journey"/> entity, or <see langword="null"/>.</value>
    public Journey? Journey { get; private set; }

    /// <summary>
    /// Gets the collection of share tokens generated for this trip.
    /// </summary>
    /// <value>A read-only collection of active and past <see cref="TripShare"/> records.</value>
    public IReadOnlyCollection<TripShare> Shares => _shares.AsReadOnly();

    private Trip()
    {
    }

    private Trip(
        Guid userId,
        string title,
        DateOnly startDate,
        DateOnly endDate,
        Visibility visibility)
    {
        UserId = userId;
        Title = title;
        StartDate = startDate;
        EndDate = endDate;
        Status = TripStatus.Draft;
        Visibility = visibility;
    }

    /// <summary>
    /// Creates a new <see cref="Trip"/> aggregate with validated date ranges.
    /// </summary>
    /// <param name="userId">The identifier of the user creating the trip.</param>
    /// <param name="title">Descriptive title for the trip.</param>
    /// <param name="startDate">Planned arrival date.</param>
    /// <param name="endDate">Planned departure date (must not precede <paramref name="startDate"/>).</param>
    /// <param name="visibility">Initial visibility level (defaults to <see cref="Visibility.Private"/>).</param>
    /// <returns>A new <see cref="Trip"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="endDate"/> is earlier than <paramref name="startDate"/>.</exception>
    public static Trip Create(
        Guid userId,
        string title,
        DateOnly startDate,
        DateOnly endDate,
        Visibility visibility = Visibility.Private)
    {
        if (endDate < startDate)
        {
            throw new ArgumentException("Trip end date cannot precede start date.", nameof(endDate));
        }

        return new Trip(
            userId,
            title ?? string.Empty,
            startDate,
            endDate,
            visibility);
    }

    /// <summary>
    /// Updates the display title of the trip.
    /// </summary>
    /// <param name="title">The revised title string.</param>
    public void UpdateTitle(string title)
    {
        Title = title ?? string.Empty;
    }

    /// <summary>
    /// Modifies the start and end calendar dates of the trip.
    /// </summary>
    /// <param name="startDate">The new start date.</param>
    /// <param name="endDate">The new end date (must not precede <paramref name="startDate"/>).</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="endDate"/> is earlier than <paramref name="startDate"/>.</exception>
    public void Reschedule(DateOnly startDate, DateOnly endDate)
    {
        if (endDate < startDate)
        {
            throw new ArgumentException("Trip end date cannot precede start date.", nameof(endDate));
        }

        StartDate = startDate;
        EndDate = endDate;
    }

    /// <summary>
    /// Modifies the access visibility level of the trip.
    /// </summary>
    /// <param name="visibility">The new <see cref="Visibility"/> setting.</param>
    public void UpdateVisibility(Visibility visibility)
    {
        Visibility = visibility;
    }

    /// <summary>
    /// Transitions the trip to a new lifecycle status.
    /// </summary>
    /// <param name="status">The target <see cref="TripStatus"/>.</param>
    public void ChangeStatus(TripStatus status)
    {
        Status = status;
    }

    /// <summary>
    /// Associates a generated journey itinerary with this trip.
    /// </summary>
    /// <param name="journeyId">The identifier of the journey to attach.</param>
    public void AttachJourney(Guid journeyId)
    {
        JourneyId = journeyId;
    }

    /// <summary>
    /// Generates a new shareable access token granting access to this trip.
    /// </summary>
    /// <param name="expiresAt">Optional expiration timestamp in UTC.</param>
    /// <returns>The created <see cref="TripShare"/> entity.</returns>
    public TripShare Share(DateTime? expiresAt = null)
    {
        var share = TripShare.Create(Id, expiresAt);
        _shares.Add(share);

        if (Visibility == Visibility.Private)
        {
            Visibility = Visibility.Shared;
        }

        return share;
    }

    /// <summary>
    /// Revokes an existing active share token.
    /// </summary>
    /// <param name="token">The token string to revoke.</param>
    public void RevokeShare(string token)
    {
        var share = _shares.FirstOrDefault(s => s.Token == token);
        share?.Revoke();
    }
}

