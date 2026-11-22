using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Queries.GetGuideRequestById;

public sealed class GetGuideRequestByIdQueryHandler : IRequestHandler<GetGuideRequestByIdQuery, Result<GuideRequestDetailsDto>>
{
    private readonly IGuideRequestReadRepository _readRepository;
    private readonly ICurrentUser _currentUser;

    public GetGuideRequestByIdQueryHandler(IGuideRequestReadRepository readRepository, ICurrentUser currentUser)
    {
        _readRepository = readRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<GuideRequestDetailsDto>> Handle(
        GetGuideRequestByIdQuery query, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result.Failure<GuideRequestDetailsDto>(GuideErrors.NotAuthenticated);

        var model = await _readRepository.GetRequestAsync(query.RequestId, cancellationToken);
        if (model is null)
            return Result.Failure<GuideRequestDetailsDto>(GuideRequestErrors.RequestNotFound);

        if (userId != model.Request.TouristId && userId != model.GuideUserId)
            return Result.Failure<GuideRequestDetailsDto>(GuideRequestErrors.NotRequestOwner);

        return model.Request;
    }
}
