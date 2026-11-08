using Voyara.Domain.Entities.TripAndJourney;

namespace Voyara.Domain.Repositories.TripAndJourney;

/// <summary>
/// Persistence contract for the Journey aggregate root.
/// </summary>
public interface IJourneyRepository
{
    /// <summary>
    /// Gets a journey by its unique identifier, including its daily itineraries and stops.
    /// </summary>
    /// <param name="id">The unique journey identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The journey aggregate root, or <see langword="null"/> if not found.</returns>
    Task<Journey?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new journey aggregate for insertion.
    /// </summary>
    /// <param name="journey">The journey aggregate root to add.</param>
    void Add(Journey journey);

    /// <summary>
    /// Checks whether a journey with the specified identifier exists.
    /// </summary>
    /// <param name="id">The unique journey identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see langword="true"/> if the journey exists; otherwise, <see langword="false"/>.</returns>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
