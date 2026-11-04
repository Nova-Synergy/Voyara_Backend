using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.DTOs.Identity;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Queries.Tourists.GetTouristById;

public sealed class GetTouristByIdQueryHandler
    : IRequestHandler<GetTouristByIdQuery, Result<TouristReadDto>>
{
    private readonly ITouristProfileRepository _tourists;
    private readonly ICurrentUser _currentUser;

    public GetTouristByIdQueryHandler(
        ITouristProfileRepository tourists,
        ICurrentUser currentUser)
    {
        _tourists = tourists;
        _currentUser = currentUser;
    }

    public async Task<Result<TouristReadDto>> Handle(
        GetTouristByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsInRole(Roles.Admin)
            && _currentUser.UserId != request.UserId)
        {
            return Result.Failure<TouristReadDto>(
                Error.Unauthorized(
                    "Identity.Forbidden",
                    "You are not allowed to access this account."));
        }

        var profile = await _tourists.GetByUserIdAsync(
            request.UserId,
            asNoTracking: true,
            cancellationToken);

        return profile is null
            ? Result.Failure<TouristReadDto>(
                Error.NotFound(
                    "Identity.NotFound",
                    "Tourist was not found."))
            : TouristReadDto.FromEntity(profile);
    }
}