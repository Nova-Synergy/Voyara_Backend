namespace Voyara.Api.Contracts.Planning;

/// <summary>
/// Request contract for generating a full cost estimate for a journey itinerary.
/// </summary>
/// <param name="Currency">Target ISO currency code (defaults to USD).</param>
/// <param name="LodgingTier">Optional lodging tier preference (e.g. Budget, Standard, Luxury).</param>
public sealed record GenerateCostEstimateRequest(
    string? Currency = "USD",
    string? LodgingTier = "Standard");
