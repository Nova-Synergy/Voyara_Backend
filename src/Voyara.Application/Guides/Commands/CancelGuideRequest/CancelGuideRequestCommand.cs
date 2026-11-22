using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Guides;

namespace Voyara.Application.Guides.Commands.CancelGuideRequest;

/// <summary>Cancels a request owned by the current tourist (pending or accepted, per the domain lifecycle).</summary>
[AuthorizeRequest]
public sealed record CancelGuideRequestCommand(Guid RequestId) : ICommand<GuideRequestDetailsDto>;
