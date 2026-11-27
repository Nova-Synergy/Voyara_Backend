using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs.Guides;
using Voyara.Application.Guides.Queries.GetGuideReviews;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Queries.GetGuideReviewById;

public sealed class GetGuideReviewByIdQueryHandler : IRequestHandler<GetGuideReviewByIdQuery, Result<GuideReviewDto>>
{
    private readonly IGuideRequestReadRepository _readRepository;
    private readonly ICurrentUser _currentUser;

    public GetGuideReviewByIdQueryHandler(IGuideRequestReadRepository readRepository, ICurrentUser currentUser)
    {
        _readRepository = readRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<GuideReviewDto>> Handle(GetGuideReviewByIdQuery query, CancellationToken cancellationToken)
    {
        var review = await _readRepository.GetReviewAsync(query.ReviewId, cancellationToken);
        if (review is null)
            return Result.Failure<GuideReviewDto>(GuideRequestErrors.ReviewNotFound);

        var target = await _readRepository.GetReviewTargetAsync(review.GuideProfileId, cancellationToken);
        if (!GuideReviewVisibility.IsVisible(target, _currentUser))
            return Result.Failure<GuideReviewDto>(GuideRequestErrors.ReviewNotFound);

        return review;
    }
}
