
using Voyara.Domain.Entities.Community;

namespace Voyara.Domain.Repositories.Community;

/// <summary>
/// Defines persistence operations for review aggregates.
/// </summary>
public interface IReviewRepository
{
    /// <summary>
    /// Retrieves a review aggregate by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the review to retrieve.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains the review aggregate if found; otherwise, <see langword="null"/>.
    /// </returns>
    Task<Review?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all review aggregates associated with a specific user asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose reviews are being retrieved.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains a read-only list of reviews created by the specified user.
    /// </returns>
    Task<IReadOnlyList<Review>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new review aggregate to the persistence context.
    /// </summary>
    /// <param name="review">The review aggregate to add.</param>
    void Add(Review review);
}
