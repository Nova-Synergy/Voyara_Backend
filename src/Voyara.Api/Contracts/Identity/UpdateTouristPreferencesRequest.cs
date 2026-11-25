using System.Text.Json.Serialization;
using Voyara.Domain.Enums;

namespace Voyara.Api.Contracts.Identity;

/// <summary>Request body for replacing the tourist's preferences; the budget is sent by name (Economy, Standard, Premium).</summary>
public sealed record UpdateTouristPreferencesRequest(
    string PreferredLanguage,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] BudgetLevel Budget,
    IReadOnlyCollection<string> InterestCodes);
