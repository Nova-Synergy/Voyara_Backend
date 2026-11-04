using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Identity;

namespace Voyara.Application.Identity.Commands.TourGuides.RegisterTourGuide;

/// <summary>Registers a tour guide: creates the account (TourGuide role) and a Pending profile.</summary>
public sealed record RegisterTourGuideCommand(
    string FullName,
    string Email,
    string Password,
    int CityId,
    string Bio,
    string? PhotoUrl,
    int YearsOfExperience) : ICommand<TourGuideReadDto>;
