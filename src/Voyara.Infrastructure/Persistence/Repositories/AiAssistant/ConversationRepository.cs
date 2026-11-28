using Microsoft.EntityFrameworkCore;
using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.AiAssistant;

namespace Voyara.Infrastructure.Persistence.Repositories.AiAssistant;

/// <summary>
/// EF Core implementation of <see cref="IConversationRepository"/>.
/// </summary>
public sealed class ConversationRepository : IConversationRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>
    /// Initializes a new instance of <see cref="ConversationRepository"/>.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ConversationRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Conversation>()
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Conversation>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Conversation>()
            .Include(c => c.Messages)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.StartedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Conversation>> GetPagedByUserIdAsync(
        Guid userId,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Conversation>()
            .AsNoTracking()
            .Include(c => c.Messages)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.StartedAt)
            .Skip(Math.Max(0, skip))
            .Take(Math.Max(1, take))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _context.Set<Conversation>()
            .AsNoTracking()
            .CountAsync(c => c.UserId == userId, cancellationToken);
    }

    /// <inheritdoc />
    public void Add(Conversation conversation)
    {
        _context.Set<Conversation>().Add(conversation);
    }

    /// <inheritdoc />
    public void Update(Conversation conversation)
    {
        _context.Set<Conversation>().Update(conversation);
    }
}
