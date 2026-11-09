namespace Voyara.Api.Contracts.Planning;

/// <summary>
/// HTTP request contract for generating a personalized AI journey itinerary.
/// </summary>
public sealed record GenerateJourneyRequest(
    string? Title,
    int DurationDays,
    IReadOnlyList<string>? Interests,
    string? TravelStyle,
    string? TransportPreference,
    string? Country,
    decimal? BudgetAmount,
    string? BudgetCurrency);
