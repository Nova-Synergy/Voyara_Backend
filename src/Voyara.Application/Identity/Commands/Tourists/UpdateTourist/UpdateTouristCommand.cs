using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Identity;
using Voyara.Domain.Enums;

namespace Voyara.Application.Identity.Commands.Tourists.UpdateTourist;

/// <summary>Updates a tourist's name and profile. Owner or admin.</summary>
[AuthorizeRequest]
public sealed record UpdateTouristCommand(
    Guid UserId,
    string FullName,
    string NationalityCode,
    string PreferredLanguage,
    BudgetLevel Budget,
    string? PhotoUrl) : ICommand<TouristReadDto>;
