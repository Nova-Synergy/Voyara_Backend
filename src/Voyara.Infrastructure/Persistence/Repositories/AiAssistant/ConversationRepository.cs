using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.AiAssistant;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.AiAssistant;

namespace Voyara.Infrastructure.Persistence.Repositories.AiAssistant;

/// <summary>EF Core implementation of the conversation repository.</summary>
public sealed class ConversationRepository : IConversationRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>Creates the repository.</summary>
    public ConversationRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Conversations
            .FirstOrDefaultAsync(conversation => conversation.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Conversation?> GetByIdWithMessagesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Conversations
            .AsNoTracking()
            .Include(conversation => conversation.Messages)
            .FirstOrDefaultAsync(conversation => conversation.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Conversation>> ListByUserAsync(
        Guid userId,
        ConversationState? state,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await FilterByUser(_context.Conversations.AsNoTracking(), userId, state)
            .OrderByDescending(conversation => conversation.StartedAt)
            .ThenByDescending(conversation => conversation.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> CountByUserAsync(
        Guid userId,
        ConversationState? state,
        CancellationToken cancellationToken = default)
    {
        return FilterByUser(_context.Conversations.AsNoTracking(), userId, state)
            .CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<Guid, int>> GetMessageCountsAsync(
        IReadOnlyCollection<Guid> conversationIds,
        CancellationToken cancellationToken = default)
    {
        if (conversationIds.Count == 0)
        {
            return new Dictionary<Guid, int>();
        }

        var ids = conversationIds.ToArray();

        return await _context.Messages
            .AsNoTracking()
            .Where(message => ids.Contains(message.ConversationId))
            .GroupBy(message => message.ConversationId)
            .Select(group => new { ConversationId = group.Key, Count = group.Count() })
            .ToDictionaryAsync(item => item.ConversationId, item => item.Count, cancellationToken);
    }

    /// <inheritdoc />
    public void Add(Conversation conversation)
    {
        _context.Conversations.Add(conversation);
    }

    private static IQueryable<Conversation> FilterByUser(
        IQueryable<Conversation> query,
        Guid userId,
        ConversationState? state)
        => state is { } value
            ? query.Where(conversation => conversation.UserId == userId && conversation.State == value)
            : query.Where(conversation => conversation.UserId == userId
                                          && conversation.State != ConversationState.Archived);
}
