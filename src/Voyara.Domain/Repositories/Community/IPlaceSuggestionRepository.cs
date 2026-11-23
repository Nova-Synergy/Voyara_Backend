
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
    /// Retrieves all place suggestion aggregates matching the specified status asynchronously.
    /// </summary>
    /// <param name="status">The suggestion status filter to apply.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains a read-only list of place suggestions matching the given status.
    /// </returns>
    Task<IReadOnlyList<PlaceSuggestion>> GetByStatusAsync(
        SuggestionStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new place suggestion aggregate to the persistence context.
    /// </summary>
    /// <param name="suggestion">The place suggestion aggregate to add.</param>
    void Add(PlaceSuggestion suggestion);
}
