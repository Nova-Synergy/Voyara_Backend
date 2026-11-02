
using Voyara.Domain.Entities.Community;

namespace Voyara.Domain.Repositories.Community;

/// <summary>
/// Defines persistence operations for user favorites.
/// </summary>
public interface IFavoriteRepository
{
    /// <summary>
    /// Retrieves a specific favorite entity matching the provided user and destination identifiers asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="destinationId">The unique identifier of the destination.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains the favorite entity if found; otherwise, <see langword="null"/>.
    /// </returns>
    Task<Favorite?> GetByUserAndDestinationAsync(
        Guid userId,
        Guid destinationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all favorite entities associated with a specific user asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose favorites are being retrieved.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains a read-only list of favorites associated with the specified user.
    /// </returns>
    Task<IReadOnlyList<Favorite>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new favorite entity to the persistence context.
    /// </summary>
    /// <param name="favorite">The favorite entity to add.</param>
    void Add(Favorite favorite);
}
