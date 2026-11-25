using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Identity.Common;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Commands.RemoveMyProfileImage;

/// <summary>Handles removal of the current tourist's profile image.</summary>
public sealed class RemoveMyProfileImageCommandHandler : IRequestHandler<RemoveMyProfileImageCommand, Result>
{
    private readonly ITouristProfileRepository _profileRepository;
    private readonly IFileStorage _fileStorage;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Creates the handler.</summary>
    public RemoveMyProfileImageCommandHandler(
        ITouristProfileRepository profileRepository,
        IFileStorage fileStorage,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _profileRepository = profileRepository;
        _fileStorage = fileStorage;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(RemoveMyProfileImageCommand request, CancellationToken cancellationToken)
    {
        var loaded = await TouristProfileAccess.LoadCurrentAsync(
            _currentUser, _profileRepository, readOnly: false, cancellationToken);
        if (loaded.IsFailure)
        {
            return Result.Failure(loaded.Error);
        }

        var profile = loaded.Value;
        var previousPhoto = profile.PhotoUrl;
        if (previousPhoto is null)
        {
            return Result.Success();
        }

        profile.ChangePhoto(null);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (ProfileImageRules.IsStoredFile(previousPhoto))
        {
            await _fileStorage.DeleteAsync(previousPhoto, CancellationToken.None);
        }

        return Result.Success();
    }
}
