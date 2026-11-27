using MediatR;
using Voyara.Application.Moderation.DTOs;
using Voyara.Domain.Repositories.Moderation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Moderation.Queries.GetAuditLogsByActor;

/// <summary>
/// Handles retrieving audit-log entries recorded for actions performed by a specific actor.
/// </summary>
public sealed class GetAuditLogsByActorQueryHandler
    : IRequestHandler<GetAuditLogsByActorQuery, Result<IReadOnlyList<AuditLogResponse>>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAuditLogsByActorQueryHandler"/> class.
    /// </summary>
    /// <param name="auditLogRepository">The repository used to query audit-log entries.</param>
    public GetAuditLogsByActorQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    /// <summary>
    /// Processes the <see cref="GetAuditLogsByActorQuery"/> to retrieve and map
    /// audit-log entries performed by the specified actor.
    /// </summary>
    /// <param name="request">The query containing the identifier of the actor whose audit logs are requested.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing a read-only list of <see cref="AuditLogResponse"/> DTOs
    /// for the specified actor.
    /// </returns>
    public async Task<Result<IReadOnlyList<AuditLogResponse>>> Handle(
        GetAuditLogsByActorQuery request,
        CancellationToken cancellationToken)
    {
        var auditLogs = await _auditLogRepository.GetByActorIdAsync(
            request.ActorId,
            cancellationToken);

        var response = auditLogs
            .Select(AuditLogResponse.FromEntity)
            .ToList();

        return Result.Success<IReadOnlyList<AuditLogResponse>>(response);
    }
}