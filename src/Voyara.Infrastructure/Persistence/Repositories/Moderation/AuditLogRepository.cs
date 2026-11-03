using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Moderation;
using Voyara.Domain.Repositories.Moderation;

namespace Voyara.Infrastructure.Persistence.Repositories.Moderation;

/// <summary>
/// Provides the Entity Framework Core implementation of the <see cref="IAuditLogRepository"/> interface.
/// </summary>
public sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditLogRepository"/> class.
    /// </summary>
    /// <param name="context">The database context used for persistence operations.</param>
    public AuditLogRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AuditLog>> GetByTargetAsync(
        string targetType,
        string targetId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .Where(log =>
                log.TargetType == targetType &&
                log.TargetId == targetId)
            .OrderByDescending(log => log.Timestamp)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AuditLog>> GetByActorIdAsync(
        Guid actorId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .Where(log => log.ActorId == actorId)
            .OrderByDescending(log => log.Timestamp)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Add(AuditLog auditLog)
    {
        _context.AuditLogs.Add(auditLog);
    }
}