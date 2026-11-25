
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.Moderation.DTOs;

namespace Voyara.Application.Moderation.Queries.GetAuditLogsByTarget;

/// <summary>
/// Retrieves audit-log entries associated with a target entity.
/// </summary>
[AuthorizeRequest(Roles.Admin)]
public sealed record GetAuditLogsByTargetQuery(
    string TargetType,
    string TargetId) : IQuery<IReadOnlyList<AuditLogResponse>>;
