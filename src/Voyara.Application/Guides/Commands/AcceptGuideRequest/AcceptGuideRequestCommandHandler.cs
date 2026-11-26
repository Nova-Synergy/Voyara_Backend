using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs.Guides;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Commands.AcceptGuideRequest;

public sealed class AcceptGuideRequestCommandHandler
    : IRequestHandler<AcceptGuideRequestCommand, Result<GuideRequestDetailsDto>>
{
    private readonly IGuideProfileRepository _profiles;
    private readonly IGuideRequestRepository _requests;
    private readonly IGuideRequestReadRepository _readRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTime _dateTime;
    private readonly IUnitOfWork _unitOfWork;

    public AcceptGuideRequestCommandHandler(
        IGuideProfileRepository profiles,
        IGuideRequestRepository requests,
        IGuideRequestReadRepository readRepository,
        ICurrentUser currentUser,
        IDateTime dateTime,
        IUnitOfWork unitOfWork)
    {
        _profiles = profiles;
        _requests = requests;
        _readRepository = readRepository;
        _currentUser = currentUser;
        _dateTime = dateTime;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GuideRequestDetailsDto>> Handle(
        AcceptGuideRequestCommand command, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result.Failure<GuideRequestDetailsDto>(GuideErrors.NotAuthenticated);

        // The per-guide lock (held until the pipeline transaction ends) serialises concurrent accepts,
        // so the overlap check below cannot be raced by another accept for the same guide.
        var loaded = await GuideRequestWorkflow.LoadForGuideAsync(
            command.RequestId, userId, _profiles, _requests, lockGuide: true, cancellationToken);
        if (loaded.IsFailure)
            return Result.Failure<GuideRequestDetailsDto>(loaded.Error);

        var (profile, request) = (loaded.Value.Profile, loaded.Value.Request);

        if (profile.Status != GuideStatus.Approved)
            return Result.Failure<GuideRequestDetailsDto>(GuideRequestErrors.GuideNotEligible);

        // Status/expiry rules belong to GuideRequest.Accept; the overlap check only matters for a pending request.
        if (request.Status == RequestStatus.Pending)
        {
            var taken = await _requests.ListTakenSlotsAsync(profile.Id, request.Date, cancellationToken);
            if (taken.Any(s => GuideRequestScheduling.Overlaps(
                    request.StartTime, request.DurationHours, s.StartTime, s.DurationHours)))
            {
                return Result.Failure<GuideRequestDetailsDto>(GuideRequestErrors.SlotConflict);
            }
        }

        var now = _dateTime.UtcNow;
        var accept = GuideDomainCall.Execute(() =>
        {
            request.Accept(command.MeetingLocation, command.AgreedPrice, command.Currency, now);

            // Chat becomes available on acceptance; the aggregate refuses to open it for any other status.
            request.OpenChatThread();
        });

        if (accept.IsFailure)
            return Result.Failure<GuideRequestDetailsDto>(accept.Error);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GuideRequestWorkflow.ReadAsync(_readRepository, request.Id, cancellationToken);
    }
}
