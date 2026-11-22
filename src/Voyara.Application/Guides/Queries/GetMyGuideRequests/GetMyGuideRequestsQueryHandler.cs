using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Queries.GetMyGuideRequests;

public sealed class GetMyGuideRequestsQueryHandler
    : IRequestHandler<GetMyGuideRequestsQuery, Result<PagedResult<GuideRequestListItemDto>>>
{
    private readonly IGuideRequestReadRepository _readRepository;
    private readonly ICurrentUser _currentUser;

    public GetMyGuideRequestsQueryHandler(IGuideRequestReadRepository readRepository, ICurrentUser currentUser)
    {
        _readRepository = readRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<GuideRequestListItemDto>>> Handle(
        GetMyGuideRequestsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result.Failure<PagedResult<GuideRequestListItemDto>>(GuideErrors.NotAuthenticated);

        return await _readRepository.ListForTouristAsync(userId, request.Status, request.Page, request.PageSize, cancellationToken);
    }
}
