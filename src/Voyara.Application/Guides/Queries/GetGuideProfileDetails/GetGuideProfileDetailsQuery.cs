using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Guide;

namespace Voyara.Application.Guides.Queries.GetGuideProfileDetails;

/// <summary>
/// Profile details. Approved profiles are public; any other status is visible only to the owner and admins
/// and is otherwise reported as not found.
/// </summary>
public sealed record GetGuideProfileDetailsQuery(Guid GuideProfileId) : IQuery<GuideProfileDetailsDto>;
