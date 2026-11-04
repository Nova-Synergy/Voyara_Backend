using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.DTOs.Identity;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Queries.TourGuides.GetTourGuideById;

public sealed class GetTourGuideByIdQueryHandler
    : IRequestHandler<GetTourGuideByIdQuery, Result<TourGuideReadDto>>
{
    private readonly IGuideProfileRepository _guides;
    private readonly ICurrentUser _currentUser;

    public GetTourGuideByIdQueryHandler(
        IGuideProfileRepository guides,
        ICurrentUser currentUser)
    {
        _guides = guides;
        _currentUser = currentUser;
    }

    public async Task<Result<TourGuideReadDto>> Handle(
        GetTourGuideByIdQuery request,
        CancellationToken cancellationToken)
    {
        var profile = await _guides.GetByIdAsync(
            request.Id,
            asNoTracking: true,
            cancellationToken);

        var isAdminOrSelf = profile is not null
            && (_currentUser.IsInRole(Roles.Admin)
                || _currentUser.UserId == profile.UserId);

        if (profile is null
            || (profile.Status != GuideStatus.Approved && !isAdminOrSelf))
        {
            return Result.Failure<TourGuideReadDto>(
                Error.NotFound(
                    "Identity.NotFound",
                    "Tour guide was not found."));
        }

        return TourGuideReadDto.FromEntity(profile);
    }
}