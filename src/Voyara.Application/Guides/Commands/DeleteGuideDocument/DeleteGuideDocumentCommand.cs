using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Guides.Commands.DeleteGuideDocument;

/// <summary>Deletes one of the authenticated user's own verification documents.</summary>
[AuthorizeRequest]
public sealed record DeleteGuideDocumentCommand(Guid DocumentId) : ICommand;
