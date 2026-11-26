using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Guides;

namespace Voyara.Application.Guides.Commands.CompleteGuideRequest;

/// <summary>Marks an accepted request (its session) as completed, which enables the tourist's review.</summary>
[AuthorizeRequest(Roles.TourGuide)]
public sealed record CompleteGuideRequestCommand(Guid RequestId) : ICommand<GuideRequestDetailsDto>;
