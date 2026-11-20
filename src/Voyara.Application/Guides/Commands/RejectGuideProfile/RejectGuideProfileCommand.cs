using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Guides.Commands.RejectGuideProfile;

/// <summary>Rejects a pending guide profile with a mandatory reason (admin only).</summary>
[AuthorizeRequest(Roles.Admin)]
public sealed record RejectGuideProfileCommand(Guid GuideProfileId, string Reason) : ICommand;
