using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Guides;
using Voyara.Domain.Repositories.Moderation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Commands.SuspendGuideProfile;

public sealed class SuspendGuideProfileCommandHandler : IRequestHandler<SuspendGuideProfileCommand, Result>
{
    private readonly IGuideProfileRepository _profiles;
    private readonly IAuditLogRepository _auditLogs;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public SuspendGuideProfileCommandHandler(
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

    public async Task<Result> Handle(SuspendGuideProfileCommand request, CancellationToken cancellationToken)
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

        if (profile.Status != GuideStatus.Approved)
        {
            return Result.Failure(GuideErrors.InvalidStatus("suspend", profile.Status.ToString()));
        }

        var suspend = GuideDomainCall.Execute(profile.Suspend);
        if (suspend.IsFailure)
        {
            return suspend;
        }

        var audit = GuideAudit.Create(adminId, "GuideProfile.Suspended", profile.Id, new { profile.UserId });
        if (audit.IsFailure)
        {
            return Result.Failure(audit.Error);
        }

        _auditLogs.Add(audit.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
