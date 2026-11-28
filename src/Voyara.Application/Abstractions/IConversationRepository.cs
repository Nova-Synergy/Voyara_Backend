using Voyara.Domain.Entities.AiAssistant;

namespace Voyara.Application.Abstractions;

/// <summary>
/// Persistence contract for managing conversational AI assistant planning sessions.
/// </summary>
public interface IConversationRepository
{
    /// <summary>
    /// Gets a conversation session by its identifier, including its message history.
    /// </summary>
    /// <param name="id">The unique conversation identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The conversation aggregate, or <see langword="null"/> if not found.</returns>
    Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all conversation sessions belonging to a specific user, ordered by newest first.
    /// </summary>
    /// <param name="userId">The unique user identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of conversation aggregates.</returns>
    Task<IReadOnlyList<Conversation>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a page of conversation sessions belonging to a specific user, ordered by newest first.
    /// </summary>
    Task<IReadOnlyList<Conversation>> GetPagedByUserIdAsync(
        Guid userId,
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts all conversation sessions belonging to a specific user.
    /// </summary>
    Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new conversation session to persistence.
    /// </summary>
    /// <param name="conversation">The conversation aggregate to insert.</param>
    void Add(Conversation conversation);

    /// <summary>
    /// Updates an existing conversation session.
    /// </summary>
    /// <param name="conversation">The conversation aggregate to update.</param>
    void Update(Conversation conversation);
}
