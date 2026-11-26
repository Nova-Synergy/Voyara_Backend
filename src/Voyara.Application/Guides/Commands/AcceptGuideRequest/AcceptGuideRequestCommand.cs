using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Guides;

namespace Voyara.Application.Guides.Commands.AcceptGuideRequest;

/// <summary>Accepts a pending request addressed to the current guide, creating its session and chat thread atomically.</summary>
[AuthorizeRequest(Roles.TourGuide)]
public sealed record AcceptGuideRequestCommand(
    Guid RequestId,
    string MeetingLocation,
    decimal AgreedPrice,
    string Currency) : ICommand<GuideRequestDetailsDto>;
