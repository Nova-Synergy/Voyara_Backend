using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Guides;

namespace Voyara.Application.Guides.Commands.RejectGuideRequest;

/// <summary>Rejects a pending request addressed to the current guide.</summary>
[AuthorizeRequest(Roles.TourGuide)]
public sealed record RejectGuideRequestCommand(Guid RequestId, string Reason) : ICommand<GuideRequestDetailsDto>;
