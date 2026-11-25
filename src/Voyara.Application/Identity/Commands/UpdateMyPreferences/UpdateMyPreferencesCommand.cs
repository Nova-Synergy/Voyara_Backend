using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;
using Voyara.Domain.Enums;

namespace Voyara.Application.Identity.Commands.UpdateMyPreferences;

/// <summary>
/// Replaces the travel preferences of the signed-in tourist: preferred language, budget and
/// the complete set of liked interests (an empty set clears them).
/// </summary>
[AuthorizeRequest(Roles.Tourist)]
public sealed record UpdateMyPreferencesCommand(
    string PreferredLanguage,
    BudgetLevel Budget,
    IReadOnlyCollection<string> InterestCodes) : ICommand<TouristProfileResponse>;
