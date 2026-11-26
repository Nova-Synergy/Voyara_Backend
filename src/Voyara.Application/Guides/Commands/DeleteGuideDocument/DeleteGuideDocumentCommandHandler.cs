using MediatR;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<DeleteGuideDocumentCommandHandler> _logger;

    public DeleteGuideDocumentCommandHandler(
        IGuideProfileRepository profiles,
        IGuideDocumentRepository documents,
        IFileStorage fileStorage,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork,
        ILogger<DeleteGuideDocumentCommandHandler> logger)
    {
        _profiles = profiles;
        _documents = documents;
        _fileStorage = fileStorage;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _logger = logger;
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

        // The record is already gone, so the document can no longer be downloaded through the API. If the file cannot be
        // removed now, it is NOT harmless (it is a private verification document): log it and let the reconciliation
        // job (ReconcileGuideDocumentFilesCommand) retry until the file is gone.
        try
        {
            await _fileStorage.DeleteAsync(storedFileName, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(
                exception,
                "Could not delete guide document file {FileName}; it will be removed by reconciliation.",
                storedFileName);
        }

        return Result.Success();
    }
}
