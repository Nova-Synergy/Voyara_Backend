using Voyara.Domain.Entities.TripAndJourney;

namespace Voyara.Domain.Repositories.TripAndJourney;

/// <summary>
/// Persistence contract for the <see cref="Trip"/> aggregate root and its associated shares.
/// </summary>
public interface ITripRepository
{
    /// <summary>
    /// Retrieves a trip by its unique identifier including shares and journey navigation.
    /// </summary>
    /// <param name="id">The unique identifier of the trip.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The <see cref="Trip"/> entity if found; otherwise, <see langword="null"/>.</returns>
    Task<Trip?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all trips owned by a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the owning user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only collection of trips owned by the user.</returns>
    Task<IReadOnlyList<Trip>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches and pages trips owned by a specific user with an optional status filter.
    /// </summary>
    /// <param name="userId">The unique identifier of the owning user.</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="skip">Number of records to skip.</param>
    /// <param name="take">Number of records to take.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple of items and total count.</returns>
    Task<(IReadOnlyList<Trip> Items, int TotalCount)> GetPagedByUserIdAsync(
        Guid userId,
        Voyara.Domain.Enums.TripStatus? status,
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a trip by an active share token.
    /// </summary>
    /// <param name="token">The URL-safe random token string.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The shared <see cref="Trip"/> if active; otherwise, <see langword="null"/>.</returns>
    Task<Trip?> GetByShareTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether a trip exists with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the trip.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see langword="true"/> if the trip exists; otherwise, <see langword="false"/>.</returns>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new trip aggregate to the change tracker.
    /// </summary>
    /// <param name="trip">The trip instance to add.</param>
    void Add(Trip trip);

    /// <summary>
    /// Updates an existing trip aggregate in the change tracker.
    /// </summary>
    /// <param name="trip">The trip instance to update.</param>
    void Update(Trip trip);

    /// <summary>
    /// Removes a trip aggregate from the persistent store.
    /// </summary>
    /// <param name="trip">The trip instance to remove.</param>
    void Delete(Trip trip);
}
