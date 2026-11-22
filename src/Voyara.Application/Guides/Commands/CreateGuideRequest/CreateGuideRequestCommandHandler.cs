using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs.Guides;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Commands.CreateGuideRequest;

public sealed class CreateGuideRequestCommandHandler
    : IRequestHandler<CreateGuideRequestCommand, Result<GuideRequestDetailsDto>>
{
    private readonly IGuideRequestRepository _requests;
    private readonly IGuideRequestReadRepository _readRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTime _dateTime;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGuideRequestCommandHandler(
        IGuideRequestRepository requests,
        IGuideRequestReadRepository readRepository,
        ICurrentUser currentUser,
        IDateTime dateTime,
        IUnitOfWork unitOfWork)
    {
        _requests = requests;
        _readRepository = readRepository;
        _currentUser = currentUser;
        _dateTime = dateTime;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GuideRequestDetailsDto>> Handle(
        CreateGuideRequestCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result.Failure<GuideRequestDetailsDto>(GuideErrors.NotAuthenticated);

        var guide = await _readRepository.GetBookingContextAsync(request.GuideProfileId, cancellationToken);
        if (guide is null)
            return Result.Failure<GuideRequestDetailsDto>(GuideErrors.ProfileNotFound);

        if (guide.Status != GuideStatus.Approved)
            return Result.Failure<GuideRequestDetailsDto>(GuideRequestErrors.GuideNotEligible);

        if (guide.GuideUserId == userId)
            return Result.Failure<GuideRequestDetailsDto>(GuideRequestErrors.OwnProfile);

        var now = _dateTime.UtcNow;
        if (request.Date < DateOnly.FromDateTime(now.UtcDateTime))
            return Result.Failure<GuideRequestDetailsDto>(GuideRequestErrors.DateInPast);

        if (!GuideRequestScheduling.IsWithinAvailability(guide, request.Date, request.StartTime, request.DurationHours))
            return Result.Failure<GuideRequestDetailsDto>(GuideRequestErrors.GuideUnavailable);

        if (await _requests.ExistsActiveAsync(userId, guide.GuideProfileId, request.Date, request.StartTime, cancellationToken))
            return Result.Failure<GuideRequestDetailsDto>(GuideRequestErrors.DuplicateRequest);

        var taken = await _requests.ListTakenSlotsAsync(guide.GuideProfileId, request.Date, cancellationToken);
        if (taken.Any(s => GuideRequestScheduling.Overlaps(request.StartTime, request.DurationHours, s.StartTime, s.DurationHours)))
            return Result.Failure<GuideRequestDetailsDto>(GuideRequestErrors.SlotConflict);

        GuideRequest? created = null;
        var build = GuideDomainCall.Execute(() =>
        {
            created = GuideRequest.Create(
                userId,
                guide.GuideProfileId,
                request.Date,
                request.StartTime,
                request.DurationHours,
                request.PartySize,
                request.PreferredLanguage,
                request.SpecialRequirements,
                now,
                GuideRequestPolicy.ResponseWindow);
        });

        if (build.IsFailure)
            return Result.Failure<GuideRequestDetailsDto>(build.Error);

        _requests.Add(created!);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var model = await _readRepository.GetRequestAsync(created!.Id, cancellationToken);
        return model is null
            ? Result.Failure<GuideRequestDetailsDto>(GuideRequestErrors.RequestNotFound)
            : model.Request;
    }
}
