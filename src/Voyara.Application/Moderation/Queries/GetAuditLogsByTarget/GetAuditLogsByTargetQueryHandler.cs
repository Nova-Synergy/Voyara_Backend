using MediatR;
using Voyara.Application.Moderation.DTOs;
using Voyara.Domain.Repositories.Moderation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Moderation.Queries.GetAuditLogsByTarget;

/// <summary>
/// Handles retrieving audit-log entries associated with a specific target entity.
/// </summary>
public sealed class GetAuditLogsByTargetQueryHandler
    : IRequestHandler<GetAuditLogsByTargetQuery, Result<IReadOnlyList<AuditLogResponse>>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAuditLogsByTargetQueryHandler"/> class.
    /// </summary>
    /// <param name="auditLogRepository">The repository used to query audit-log entries.</param>
    public GetAuditLogsByTargetQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    /// <summary>
    /// Processes the <see cref="GetAuditLogsByTargetQuery"/> to retrieve and map
    /// audit-log entries for the specified target.
    /// </summary>
    /// <param name="request">The query containing the target entity type and identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing a read-only list of <see cref="AuditLogResponse"/> DTOs
    /// for the requested target.
    /// </returns>
    public async Task<Result<IReadOnlyList<AuditLogResponse>>> Handle(
        GetAuditLogsByTargetQuery request,
        CancellationToken cancellationToken)
    {
        var auditLogs = await _auditLogRepository.GetByTargetAsync(
            request.TargetType.Trim(),
            request.TargetId.Trim(),
            cancellationToken);

        var response = auditLogs
            .Select(AuditLogResponse.FromEntity)
            .ToList();

        return Result.Success<IReadOnlyList<AuditLogResponse>>(response);
    }
}