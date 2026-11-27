using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Guides;

namespace Voyara.Application.Guides.Queries.GetGuideSessionById;

/// <summary>Gets a session; allowed for the two participants of its request.</summary>
[AuthorizeRequest]
public sealed record GetGuideSessionByIdQuery(Guid SessionId) : IQuery<GuideSessionDto>;
