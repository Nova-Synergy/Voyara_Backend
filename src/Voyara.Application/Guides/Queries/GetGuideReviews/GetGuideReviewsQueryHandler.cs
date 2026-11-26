using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Queries.GetGuideReviews;

public sealed class GetGuideReviewsQueryHandler
    : IRequestHandler<GetGuideReviewsQuery, Result<PagedResult<GuideReviewDto>>>
{
    private readonly IGuideRequestReadRepository _readRepository;
    private readonly ICurrentUser _currentUser;

    public GetGuideReviewsQueryHandler(IGuideRequestReadRepository readRepository, ICurrentUser currentUser)
    {
        _readRepository = readRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<GuideReviewDto>>> Handle(
        GetGuideReviewsQuery query, CancellationToken cancellationToken)
    {
        var target = await _readRepository.GetReviewTargetAsync(query.GuideProfileId, cancellationToken);
        if (!GuideReviewVisibility.IsVisible(target, _currentUser))
            return Result.Failure<PagedResult<GuideReviewDto>>(GuideErrors.ProfileNotFound);

        return await _readRepository.ListReviewsAsync(query.GuideProfileId, query.Page, query.PageSize, cancellationToken);
    }
}
