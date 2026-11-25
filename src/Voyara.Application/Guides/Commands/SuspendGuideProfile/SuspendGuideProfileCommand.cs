using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Guides.Commands.SuspendGuideProfile;

/// <summary>Suspends an approved guide profile (admin only).</summary>
[AuthorizeRequest(Roles.Admin)]
public sealed record SuspendGuideProfileCommand(Guid GuideProfileId) : ICommand;
