using Voyara.Domain.Entities.AiAssistant;
using Voyara.Domain.Enums;

namespace Voyara.Domain.Repositories.AiAssistant;

/// <summary>Persistence contract for the Conversation aggregate (a user's chat with the AI travel assistant).</summary>
public interface IConversationRepository
{
    /// <summary>Gets a tracked conversation without its messages, or null when it does not exist.</summary>
    Task<Conversation?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>Gets a read-only conversation with all of its messages, or null when it does not exist.</summary>
    Task<Conversation?> GetByIdWithMessagesAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists a user's conversations, newest first, without messages. A null state returns every state except
    /// Archived (archiving hides a conversation); pass Archived explicitly to list archived ones.
    /// </summary>
    Task<IReadOnlyList<Conversation>> ListByUserAsync(
        Guid userId,
        ConversationState? state,
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    /// <summary>Counts the conversations <see cref="ListByUserAsync"/> would return, ignoring paging.</summary>
    Task<int> CountByUserAsync(
        Guid userId,
        ConversationState? state,
        CancellationToken cancellationToken = default);

    /// <summary>Returns the number of messages of each of the given conversations (0 when it has none).</summary>
    Task<IReadOnlyDictionary<Guid, int>> GetMessageCountsAsync(
        IReadOnlyCollection<Guid> conversationIds,
        CancellationToken cancellationToken = default);

    /// <summary>Adds a new conversation to the persistence context.</summary>
    void Add(Conversation conversation);
}
