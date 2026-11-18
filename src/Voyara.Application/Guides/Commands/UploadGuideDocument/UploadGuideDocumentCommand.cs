using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Guides.Commands.UploadGuideDocument;

/// <summary>Uploads a verification document for the authenticated user's own profile.</summary>
[AuthorizeRequest]
public sealed record UploadGuideDocumentCommand(
    string DocumentType,
    string OriginalFileName,
    string ContentType,
    long Length,
    Stream Content) : ICommand<Guid>;
