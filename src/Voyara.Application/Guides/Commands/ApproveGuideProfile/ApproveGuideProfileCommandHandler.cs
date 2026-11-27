using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Guides;
using Voyara.Domain.Repositories.Moderation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Commands.ApproveGuideProfile;

/// <summary>
/// Approval, role grant and audit entry are written through one unit of work. The command pipeline
/// wraps every command in a database transaction, so they commit or roll back together (SEQ-13).
/// </summary>
public sealed class ApproveGuideProfileCommandHandler : IRequestHandler<ApproveGuideProfileCommand, Result>
{
    private readonly IGuideProfileRepository _profiles;
    private readonly IGuideRoleGranter _roleGranter;
    private readonly IAuditLogRepository _auditLogs;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveGuideProfileCommandHandler(
        IGuideProfileRepository profiles,
        IGuideRoleGranter roleGranter,
        IAuditLogRepository auditLogs,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _profiles = profiles;
        _roleGranter = roleGranter;
        _auditLogs = auditLogs;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ApproveGuideProfileCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } adminId)
        {
            return Result.Failure(GuideErrors.NotAuthenticated);
        }

        var profile = await _profiles.GetByIdAsync(request.GuideProfileId, cancellationToken);
        if (profile is null)
        {
            return Result.Failure(GuideErrors.ProfileNotFound);
        }

        if (profile.Status != GuideStatus.Pending)
        {
            return Result.Failure(GuideErrors.InvalidStatus("approve", profile.Status.ToString()));
        }

        var approve = GuideDomainCall.Execute(profile.Approve);
        if (approve.IsFailure)
        {
            return approve;
        }

        var grant = await _roleGranter.GrantTourGuideRoleAsync(profile.UserId, cancellationToken);
        if (grant.IsFailure)
        {
            return grant;
        }

        var audit = GuideAudit.Create(adminId, "GuideProfile.Approved", profile.Id, new { profile.UserId });
        if (audit.IsFailure)
        {
            return Result.Failure(audit.Error);
        }

        _auditLogs.Add(audit.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
