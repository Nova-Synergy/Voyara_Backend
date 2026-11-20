using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Repositories.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Commands.UploadGuideDocument;

public sealed class UploadGuideDocumentCommandHandler : IRequestHandler<UploadGuideDocumentCommand, Result<Guid>>
{
    private readonly IGuideProfileStore _profiles;
    private readonly IGuideDocumentStore _documents;
    private readonly IGuideDocumentPolicy _policy;
    private readonly IFileStorage _fileStorage;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UploadGuideDocumentCommandHandler(
        IGuideProfileStore profiles,
        IGuideDocumentStore documents,
        IGuideDocumentPolicy policy,
        IFileStorage fileStorage,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _profiles = profiles;
        _documents = documents;
        _policy = policy;
        _fileStorage = fileStorage;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(UploadGuideDocumentCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
        {
            return Result.Failure<Guid>(GuideErrors.NotAuthenticated);
        }

        var profile = await _profiles.GetByUserIdAsync(userId, cancellationToken);
        if (profile is null)
        {
            return Result.Failure<Guid>(GuideErrors.ProfileNotFound);
        }

        if (await _documents.CountByProfileIdAsync(profile.Id, cancellationToken) >= _policy.MaxDocumentsPerProfile)
        {
            return Result.Failure<Guid>(GuideErrors.TooManyDocuments);
        }

        var validation = await _policy.ValidateAsync(
            request.OriginalFileName, request.ContentType, request.Length, request.Content, cancellationToken);
        if (validation.IsFailure)
        {
            return Result.Failure<Guid>(validation.Error);
        }

        // The storage name is generated here; the user-supplied name is only kept as display metadata.
        var storedFileName = $"guide-doc-{Guid.NewGuid():N}{validation.Value}";
        var displayName = SanitizeDisplayName(request.OriginalFileName);

        GuideDocument? document = null;
        var build = GuideDomainCall.Execute(() => document = GuideDocument.Create(
            profile.Id, request.DocumentType, displayName, storedFileName, request.ContentType, request.Length));
        if (build.IsFailure)
        {
            return Result.Failure<Guid>(build.Error);
        }

        await _fileStorage.SaveAsync(storedFileName, request.Content, cancellationToken);

        try
        {
            _documents.Add(document!);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Do not leave an orphaned private file behind when the database write fails.
            await _fileStorage.DeleteAsync(storedFileName, CancellationToken.None);
            throw;
        }

        return Result.Success(document!.Id);
    }

    private static string SanitizeDisplayName(string fileName)
    {
        var name = Path.GetFileName(fileName.Replace('\\', '/'));
        var cleaned = new string(name.Where(c => !char.IsControl(c)).ToArray()).Trim();
        if (cleaned.Length > GuideDocument.MaxOriginalFileNameLength)
            cleaned = cleaned[^GuideDocument.MaxOriginalFileNameLength..];
        return cleaned;
    }
}
