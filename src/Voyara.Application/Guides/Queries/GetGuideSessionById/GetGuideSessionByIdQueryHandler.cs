using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Queries.GetGuideSessionById;

public sealed class GetGuideSessionByIdQueryHandler : IRequestHandler<GetGuideSessionByIdQuery, Result<GuideSessionDto>>
{
    private readonly IGuideRequestReadRepository _readRepository;
    private readonly ICurrentUser _currentUser;

    public GetGuideSessionByIdQueryHandler(IGuideRequestReadRepository readRepository, ICurrentUser currentUser)
    {
        _readRepository = readRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<GuideSessionDto>> Handle(GetGuideSessionByIdQuery query, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result.Failure<GuideSessionDto>(GuideErrors.NotAuthenticated);

        var model = await _readRepository.GetSessionAsync(query.SessionId, cancellationToken);
        if (model is null)
            return Result.Failure<GuideSessionDto>(GuideRequestErrors.SessionNotFound);

        if (userId != model.TouristUserId && userId != model.GuideUserId)
            return Result.Failure<GuideSessionDto>(GuideRequestErrors.NotRequestOwner);

        return model.Session;
    }
}
