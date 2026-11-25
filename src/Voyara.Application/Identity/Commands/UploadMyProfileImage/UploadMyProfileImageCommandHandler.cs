using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Identity.Common;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Commands.UploadMyProfileImage;

/// <summary>Handles profile image uploads: validates the real content, stores it and swaps the photo.</summary>
public sealed class UploadMyProfileImageCommandHandler : IRequestHandler<UploadMyProfileImageCommand, Result<string>>
{
    private readonly ITouristProfileRepository _profileRepository;
    private readonly IFileStorage _fileStorage;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Creates the handler.</summary>
    public UploadMyProfileImageCommandHandler(
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
    public async Task<Result<string>> Handle(UploadMyProfileImageCommand request, CancellationToken cancellationToken)
    {
        var loaded = await TouristProfileAccess.LoadCurrentAsync(
            _currentUser, _profileRepository, readOnly: false, cancellationToken);
        if (loaded.IsFailure)
        {
            return Result.Failure<string>(loaded.Error);
        }

        // The declared length and content type come from the client, so the bytes are checked here.
        using var buffer = new MemoryStream();
        await CopyBoundedAsync(request.Content, buffer, ProfileImageRules.MaxBytes + 1, cancellationToken);
        if (buffer.Length is 0 or > ProfileImageRules.MaxBytes)
        {
            return Result.Failure<string>(Error.Validation(
                "ProfileImage.InvalidSize",
                $"The image must be between 1 byte and {ProfileImageRules.MaxBytes / (1024 * 1024)} MB."));
        }

        var detected = ProfileImageRules.Detect(buffer.GetBuffer().AsSpan(0, (int)Math.Min(buffer.Length, 16)));
        if (detected is null)
        {
            return Result.Failure<string>(Error.Validation(
                "ProfileImage.InvalidFormat", "The file is not a valid JPEG, PNG or WebP image."));
        }

        var profile = loaded.Value;
        var previousPhoto = profile.PhotoUrl;
        var fileName = $"tourist-{profile.UserId:N}-{Guid.NewGuid():N}{detected.Value.Extension}";

        buffer.Position = 0;
        var storedKey = await _fileStorage.SaveAsync(fileName, buffer, cancellationToken);

        try
        {
            profile.ChangePhoto(storedKey);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await _fileStorage.DeleteAsync(storedKey, CancellationToken.None);
            throw;
        }

        if (ProfileImageRules.IsStoredFile(previousPhoto) && previousPhoto != storedKey)
        {
            await _fileStorage.DeleteAsync(previousPhoto!, CancellationToken.None);
        }

        return storedKey;
    }

    // Reads at most limit bytes so an oversized body is never fully buffered.
    private static async Task CopyBoundedAsync(Stream source, Stream target, long limit, CancellationToken cancellationToken)
    {
        var chunk = new byte[81920];
        long total = 0;

        while (total < limit)
        {
            var toRead = (int)Math.Min(chunk.Length, limit - total);
            var read = await source.ReadAsync(chunk.AsMemory(0, toRead), cancellationToken);
            if (read == 0)
            {
                break;
            }

            await target.WriteAsync(chunk.AsMemory(0, read), cancellationToken);
            total += read;
        }
    }
}
