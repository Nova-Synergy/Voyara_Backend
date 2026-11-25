using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Application.Identity.Common;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Queries.GetMyProfileImage;

/// <summary>Handles reading the current tourist's stored profile image.</summary>
public sealed class GetMyProfileImageQueryHandler : IRequestHandler<GetMyProfileImageQuery, Result<ProfileImageResponse>>
{
    private static readonly Error ImageNotFound = Error.NotFound(
        "ProfileImage.NotFound", "The tourist has no stored profile image.");

    private readonly ITouristProfileRepository _profileRepository;
    private readonly IFileStorage _fileStorage;
    private readonly ICurrentUser _currentUser;

    /// <summary>Creates the handler.</summary>
    public GetMyProfileImageQueryHandler(
        ITouristProfileRepository profileRepository,
        IFileStorage fileStorage,
        ICurrentUser currentUser)
    {
        _profileRepository = profileRepository;
        _fileStorage = fileStorage;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<ProfileImageResponse>> Handle(GetMyProfileImageQuery request, CancellationToken cancellationToken)
    {
        var loaded = await TouristProfileAccess.LoadCurrentAsync(
            _currentUser, _profileRepository, readOnly: true, cancellationToken);
        if (loaded.IsFailure)
        {
            return Result.Failure<ProfileImageResponse>(loaded.Error);
        }

        var photo = loaded.Value.PhotoUrl;
        if (!ProfileImageRules.IsStoredFile(photo))
        {
            return Result.Failure<ProfileImageResponse>(ImageNotFound);
        }

        var contentType = ProfileImageRules.ContentTypeFromFileName(photo!);
        var content = await _fileStorage.OpenReadAsync(photo!, cancellationToken);
        if (content is null || contentType is null)
        {
            if (content is not null)
            {
                await content.DisposeAsync();
            }

            return Result.Failure<ProfileImageResponse>(ImageNotFound);
        }

        return new ProfileImageResponse(content, contentType);
    }
}
