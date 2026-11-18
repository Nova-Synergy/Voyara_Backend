using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Guides.Commands.ApproveGuideProfile;

/// <summary>Approves a pending guide profile and grants the TourGuide role (admin only).</summary>
[AuthorizeRequest(Roles.Admin)]
public sealed record ApproveGuideProfileCommand(Guid GuideProfileId) : ICommand;
