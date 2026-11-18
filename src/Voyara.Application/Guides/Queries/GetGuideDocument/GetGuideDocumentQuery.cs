using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Guides;

namespace Voyara.Application.Guides.Queries.GetGuideDocument;

/// <summary>Streams a verification document to its owner or an administrator.</summary>
[AuthorizeRequest]
public sealed record GetGuideDocumentQuery(Guid DocumentId) : IQuery<GuideDocumentFile>;
