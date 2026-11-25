using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Application.Identity.Common;
using Voyara.Application.Mappings;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Commands.UpdateMyTouristProfile;

/// <summary>Handles updates of the current tourist's personal details.</summary>
public sealed class UpdateMyTouristProfileCommandHandler
    : IRequestHandler<UpdateMyTouristProfileCommand, Result<TouristProfileResponse>>
{
    private readonly ITouristProfileRepository _profileRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Creates the handler.</summary>
    public UpdateMyTouristProfileCommandHandler(
        ITouristProfileRepository profileRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _profileRepository = profileRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<TouristProfileResponse>> Handle(
        UpdateMyTouristProfileCommand request,
        CancellationToken cancellationToken)
    {
        var loaded = await TouristProfileAccess.LoadCurrentAsync(
            _currentUser, _profileRepository, readOnly: false, cancellationToken);
        if (loaded.IsFailure)
        {
            return Result.Failure<TouristProfileResponse>(loaded.Error);
        }

        var profile = loaded.Value;
        profile.User.Rename(request.FullName);
        profile.ChangeNationality(request.NationalityCode);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return profile.ToResponse();
    }
}
