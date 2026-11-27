using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Queries.GetIncomingGuideRequests;

public sealed class GetIncomingGuideRequestsQueryHandler
    : IRequestHandler<GetIncomingGuideRequestsQuery, Result<PagedResult<GuideRequestListItemDto>>>
{
    private readonly IGuideRequestReadRepository _readRepository;
    private readonly ICurrentUser _currentUser;

    public GetIncomingGuideRequestsQueryHandler(IGuideRequestReadRepository readRepository, ICurrentUser currentUser)
    {
        _readRepository = readRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<GuideRequestListItemDto>>> Handle(
        GetIncomingGuideRequestsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result.Failure<PagedResult<GuideRequestListItemDto>>(GuideErrors.NotAuthenticated);

        return await _readRepository.ListForGuideAsync(userId, request.Status, request.Page, request.PageSize, cancellationToken);
    }
}
