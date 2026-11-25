using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Application.Identity.Common;
using Voyara.Application.Mappings;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Queries.GetMyTouristProfile;

/// <summary>Handles the lookup of the current tourist's profile.</summary>
public sealed class GetMyTouristProfileQueryHandler
    : IRequestHandler<GetMyTouristProfileQuery, Result<TouristProfileResponse>>
{
    private readonly ITouristProfileRepository _profileRepository;
    private readonly ICurrentUser _currentUser;

    /// <summary>Creates the handler.</summary>
    public GetMyTouristProfileQueryHandler(ITouristProfileRepository profileRepository, ICurrentUser currentUser)
    {
        _profileRepository = profileRepository;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<TouristProfileResponse>> Handle(
        GetMyTouristProfileQuery request,
        CancellationToken cancellationToken)
    {
        var profile = await TouristProfileAccess.LoadCurrentAsync(
            _currentUser, _profileRepository, readOnly: true, cancellationToken);

        if (profile.IsFailure)
        {
            return Result.Failure<TouristProfileResponse>(profile.Error);
        }

        return Result.Success(profile.Value.ToResponse());
    }
}
