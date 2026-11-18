using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs.Guides;
using Voyara.Application.Guides.Queries.GetGuideProfileDetails;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Queries.GetMyGuideProfile;

public sealed class GetMyGuideProfileQueryHandler
    : IRequestHandler<GetMyGuideProfileQuery, Result<GuideProfileDetailsDto>>
{
    private readonly IGuideReadRepository _readRepository;
    private readonly ICurrentUser _currentUser;

    public GetMyGuideProfileQueryHandler(IGuideReadRepository readRepository, ICurrentUser currentUser)
    {
        _readRepository = readRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<GuideProfileDetailsDto>> Handle(
        GetMyGuideProfileQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
        {
            return Result.Failure<GuideProfileDetailsDto>(GuideErrors.NotAuthenticated);
        }

        var model = await _readRepository.GetDetailsByUserIdAsync(userId, cancellationToken);
        return await GuideDetailsVisibility.ApplyAsync(model, _currentUser, _readRepository, cancellationToken);
    }
}
