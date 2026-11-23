
using Voyara.Domain.Entities.Community;
using Voyara.Domain.Enums;

namespace Voyara.Domain.Repositories.Community;

/// <summary>
/// Defines persistence operations for place-suggestion aggregates.
/// </summary>
public interface IPlaceSuggestionRepository
{
    /// <summary>
    /// Retrieves a place suggestion aggregate by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the place suggestion to retrieve.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains the place suggestion aggregate if found; otherwise, <see langword="null"/>.
    /// </returns>
    Task<PlaceSuggestion?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves place suggestions belonging to a user.
    /// </summary>
    /// <param name="userId">The owner's identifier.</param>
    /// <param name="skip">The number of records to skip.</param>
    /// <param name="take">The maximum number of records to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<IReadOnlyList<PlaceSuggestion>> GetByUserAsync(
        Guid userId,
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts the active place suggestions belonging to a user.
    /// </summary>
    /// <param name="userId">The owner's identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<int> CountByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves place suggestions having the specified status.
    /// </summary>
    Task<IReadOnlyList<PlaceSuggestion>> GetByStatusAsync(
        SuggestionStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new place suggestion aggregate to the persistence context.
    /// </summary>
    /// <param name="suggestion">The place suggestion aggregate to add.</param>
    void Add(PlaceSuggestion suggestion);
}
