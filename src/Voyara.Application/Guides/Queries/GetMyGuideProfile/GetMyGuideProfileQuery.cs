using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Guides;

namespace Voyara.Application.Guides.Queries.GetMyGuideProfile;

/// <summary>Returns the authenticated user's own profile in any status.</summary>
[AuthorizeRequest]
public sealed record GetMyGuideProfileQuery : IQuery<GuideProfileDetailsDto>;
