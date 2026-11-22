using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Commands.DeleteGuideDocument;

public sealed class DeleteGuideDocumentCommandHandler : IRequestHandler<DeleteGuideDocumentCommand, Result>
{
    private readonly IGuideProfileRepository _profiles;
    private readonly IGuideDocumentRepository _documents;
    private readonly IFileStorage _fileStorage;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteGuideDocumentCommandHandler(
        IGuideProfileRepository profiles,
        IGuideDocumentRepository documents,
        IFileStorage fileStorage,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _profiles = profiles;
        _documents = documents;
        _fileStorage = fileStorage;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteGuideDocumentCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
        {
            return Result.Failure(GuideErrors.NotAuthenticated);
        }

        var profile = await _profiles.GetByUserIdAsync(userId, cancellationToken);
        var document = await _documents.GetByIdAsync(request.DocumentId, cancellationToken);

        // A foreign document is reported as not found so its existence is not revealed (IDOR).
        if (profile is null || document is null || document.GuideProfileId != profile.Id)
        {
            return Result.Failure(GuideErrors.DocumentNotFound);
        }

        var storedFileName = document.StoredFileName;
        _documents.Remove(document);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // The file is removed only after the record is gone; a failure here leaves a harmless private orphan.
        await _fileStorage.DeleteAsync(storedFileName, cancellationToken);

        return Result.Success();
    }
}
