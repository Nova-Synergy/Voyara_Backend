using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Guides;

namespace Voyara.Application.Guides.Commands.CreateGuideRequest;

/// <summary>Creates a pending request from the authenticated tourist to a guide. The tourist is never taken from the payload.</summary>
[AuthorizeRequest(Roles.Tourist)]
public sealed record CreateGuideRequestCommand(
    Guid GuideProfileId,
    DateOnly Date,
    TimeOnly StartTime,
    int DurationHours,
    int PartySize,
    string PreferredLanguage,
    string? SpecialRequirements) : ICommand<GuideRequestDetailsDto>;
