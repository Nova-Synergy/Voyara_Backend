using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Identity;

namespace Voyara.Application.Identity.Commands.TourGuides.UpdateTourGuide;

/// <summary>Updates a guide's name and profile details. Owner or admin.</summary>
[AuthorizeRequest]
public sealed record UpdateTourGuideCommand(
    Guid Id,
    string FullName,
    string Bio,
    string? PhotoUrl,
    int YearsOfExperience) : ICommand<TourGuideReadDto>;
