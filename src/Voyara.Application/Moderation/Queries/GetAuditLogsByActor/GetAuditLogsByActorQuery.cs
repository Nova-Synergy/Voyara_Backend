
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.Moderation.DTOs;

namespace Voyara.Application.Moderation.Queries.GetAuditLogsByActor;

/// <summary>
/// Retrieves audit-log entries performed by a specific administrator.
/// </summary>
[AuthorizeRequest(Roles.Admin)]
public sealed record GetAuditLogsByActorQuery(
    Guid ActorId) : IQuery<IReadOnlyList<AuditLogResponse>>;
