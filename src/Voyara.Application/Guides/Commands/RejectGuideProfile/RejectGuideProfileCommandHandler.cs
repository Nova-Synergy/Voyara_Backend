using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Guides;
using Voyara.Domain.Repositories.Moderation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Commands.RejectGuideProfile;

public sealed class RejectGuideProfileCommandHandler : IRequestHandler<RejectGuideProfileCommand, Result>
{
    private readonly IGuideProfileRepository _profiles;
    private readonly IAuditLogRepository _auditLogs;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public RejectGuideProfileCommandHandler(
        IGuideProfileRepository profiles,
        IAuditLogRepository auditLogs,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _profiles = profiles;
        _auditLogs = auditLogs;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RejectGuideProfileCommand request, CancellationToken cancellationToken)
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
            return Result.Failure(GuideErrors.InvalidStatus("reject", profile.Status.ToString()));
        }

        var reject = GuideDomainCall.Execute(() => profile.Reject(request.Reason));
        if (reject.IsFailure)
        {
            return reject;
        }

        var audit = GuideAudit.Create(
            adminId, "GuideProfile.Rejected", profile.Id, new { profile.UserId, Reason = profile.RejectionReason });
        if (audit.IsFailure)
        {
            return Result.Failure(audit.Error);
        }

        _auditLogs.Add(audit.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
