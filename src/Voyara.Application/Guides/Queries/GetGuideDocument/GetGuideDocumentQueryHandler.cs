using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.DTOs.Guides;
using Voyara.Domain.Repositories.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Queries.GetGuideDocument;

public sealed class GetGuideDocumentQueryHandler : IRequestHandler<GetGuideDocumentQuery, Result<GuideDocumentFile>>
{
    private readonly IGuideDocumentStore _documents;
    private readonly IGuideProfileStore _profiles;
    private readonly IFileStorage _fileStorage;
    private readonly ICurrentUser _currentUser;

    public GetGuideDocumentQueryHandler(
        IGuideDocumentStore documents,
        IGuideProfileStore profiles,
        IFileStorage fileStorage,
        ICurrentUser currentUser)
    {
        _documents = documents;
        _profiles = profiles;
        _fileStorage = fileStorage;
        _currentUser = currentUser;
    }

    public async Task<Result<GuideDocumentFile>> Handle(GetGuideDocumentQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
        {
            return Result.Failure<GuideDocumentFile>(GuideErrors.NotAuthenticated);
        }

        var document = await _documents.GetByIdAsync(request.DocumentId, cancellationToken);
        if (document is null)
        {
            return Result.Failure<GuideDocumentFile>(GuideErrors.DocumentNotFound);
        }

        if (!_currentUser.IsInRole(Roles.Admin))
        {
            var profile = await _profiles.GetByIdAsync(document.GuideProfileId, cancellationToken);
            if (profile is null || profile.UserId != userId)
            {
                return Result.Failure<GuideDocumentFile>(GuideErrors.DocumentNotFound);
            }
        }

        var stream = await _fileStorage.OpenReadAsync(document.StoredFileName, cancellationToken);
        if (stream is null)
        {
            return Result.Failure<GuideDocumentFile>(GuideErrors.DocumentNotFound);
        }

        return new GuideDocumentFile(stream, document.ContentType, document.OriginalFileName);
    }
}
