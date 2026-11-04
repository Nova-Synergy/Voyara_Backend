using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Identity;
using Voyara.Domain.Enums;

namespace Voyara.Application.Identity.Commands.Tourists.RegisterTourist;

/// <summary>Registers a tourist: creates the account (Tourist role) and the profile.</summary>
public sealed record RegisterTouristCommand(
    string FullName,
    string Email,
    string Password,
    string NationalityCode,
    string PreferredLanguage,
    BudgetLevel Budget,
    string? PhotoUrl) : ICommand<TouristReadDto>;
