
using Voyara.Domain.Entities.Moderation;

namespace Voyara.Domain.Repositories.Moderation;

/// <summary>
/// Defines append-only persistence operations for managing <see cref="AuditLog"/> entities.
/// </summary>
public interface IAuditLogRepository
{
    /// <summary>
    /// Retrieves a read-only list of audit logs for the specified target asynchronously.
    /// </summary>
    /// <param name="targetType">The type of the target entity.</param>
    /// <param name="targetId">The unique identifier of the target entity as a string.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A read-only list of audit logs associated with the specified target.</returns>
    Task<IReadOnlyList<AuditLog>> GetByTargetAsync(
        string targetType,
        string targetId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a read-only list of audit logs performed by the specified actor asynchronously.
    /// </summary>
    /// <param name="actorId">The unique identifier of the actor who performed the action.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A read-only list of audit logs associated with the specified actor.</returns>
    Task<IReadOnlyList<AuditLog>> GetByActorIdAsync(
        Guid actorId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new audit log entry to the repository tracking context.
    /// </summary>
    /// <param name="auditLog">The audit log entity to add.</param>
    void Add(AuditLog auditLog);
}
