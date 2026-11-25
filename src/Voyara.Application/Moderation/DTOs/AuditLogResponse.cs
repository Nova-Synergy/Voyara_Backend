using Voyara.Domain.Entities.Moderation;

namespace Voyara.Application.Moderation.DTOs;

/// <summary>
/// Represents an audit-log entry returned to an administrator.
/// </summary>
/// <param name="Id">The unique identifier of the audit-log entry.</param>
/// <param name="ActorId">The identifier of the user or administrator who performed the action.</param>
/// <param name="Action">The name or code of the action recorded in the audit log.</param>
/// <param name="TargetType">The type of entity or resource affected by the action.</param>
/// <param name="TargetId">The identifier of the affected entity or resource.</param>
/// <param name="Timestamp">The timestamp when the action occurred.</param>
/// <param name="DetailsJson">A JSON-formatted payload containing additional context about the action.</param>
public sealed record AuditLogResponse(
    long Id,
    Guid ActorId,
    string Action,
    string TargetType,
    string TargetId,
    DateTimeOffset Timestamp,
    string DetailsJson)
{
    /// <summary>
    /// Maps an <see cref="AuditLog"/> entity to an <see cref="AuditLogResponse"/> DTO.
    /// </summary>
    /// <param name="auditLog">The audit-log entity to map.</param>
    /// <returns>The populated <see cref="AuditLogResponse"/> instance.</returns>
    public static AuditLogResponse FromEntity(AuditLog auditLog)
    {
        return new AuditLogResponse(
            auditLog.Id,
            auditLog.ActorId,
            auditLog.Action,
            auditLog.TargetType,
            auditLog.TargetId,
            auditLog.Timestamp,
            auditLog.Details.Json);
    }
}