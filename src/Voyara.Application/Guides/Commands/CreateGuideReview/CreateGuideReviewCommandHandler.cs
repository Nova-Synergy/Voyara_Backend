using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs.Guides;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Commands.CreateGuideReview;

public sealed class CreateGuideReviewCommandHandler : IRequestHandler<CreateGuideReviewCommand, Result<GuideReviewDto>>
{
    private readonly IGuideProfileRepository _profiles;
    private readonly IGuideRequestRepository _requests;
    private readonly IGuideRequestReadRepository _readRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGuideReviewCommandHandler(
        IGuideProfileRepository profiles,
        IGuideRequestRepository requests,
        IGuideRequestReadRepository readRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _profiles = profiles;
        _requests = requests;
        _readRepository = readRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GuideReviewDto>> Handle(CreateGuideReviewCommand command, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result.Failure<GuideReviewDto>(GuideErrors.NotAuthenticated);

        var guideProfileId = await _requests.GetGuideProfileIdAsync(command.GuideRequestId, cancellationToken);
        if (guideProfileId is not { } profileId)
            return Result.Failure<GuideReviewDto>(GuideRequestErrors.RequestNotFound);

        // Rating statistics are a read-modify-write on the guide profile: serialise reviews per guide, and load
        // everything only after the lock is held so the statistics are computed from committed values.
        await _requests.AcquireGuideLockAsync(profileId, cancellationToken);

        var request = await _requests.GetByIdWithReviewAsync(command.GuideRequestId, cancellationToken);
        if (request is null)
            return Result.Failure<GuideReviewDto>(GuideRequestErrors.RequestNotFound);

        if (request.UserId != userId)
            return Result.Failure<GuideReviewDto>(GuideRequestErrors.NotRequestOwner);

        if (request.Status != RequestStatus.Completed)
            return Result.Failure<GuideReviewDto>(GuideRequestErrors.ReviewNotAllowedYet);

        if (request.GuideReview is not null)
            return Result.Failure<GuideReviewDto>(GuideRequestErrors.ReviewAlreadyExists);

        var profile = await _profiles.GetByIdAsync(profileId, cancellationToken);
        if (profile is null)
            return Result.Failure<GuideReviewDto>(GuideErrors.ProfileNotFound);

        GuideReview? review = null;
        var apply = GuideDomainCall.Execute(() =>
        {
            review = request.AddReview(command.Rating, command.Text);
            profile.ApplyReview(command.Rating);
        });

        if (apply.IsFailure)
            return Result.Failure<GuideReviewDto>(apply.Error);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await _readRepository.GetReviewAsync(review!.Id, cancellationToken);
        return dto is null
            ? Result.Failure<GuideReviewDto>(GuideRequestErrors.RequestNotFound)
            : dto;
    }
}
