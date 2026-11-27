using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.CostEstimation.Queries.EstimateAccommodationCost;

/// <summary>
/// Query to calculate accommodation cost estimates for a journey itinerary.
/// </summary>
/// <param name="JourneyId">The unique identifier of the journey.</param>
/// <param name="Currency">Target ISO currency code (defaults to USD).</param>
/// <param name="Tier">Optional lodging tier (e.g. "Budget", "Standard", "Luxury").</param>
public sealed record EstimateAccommodationCostQuery(
    Guid JourneyId,
    string? Currency = "USD",
    string? Tier = "Standard") : IQuery<CategoryCostEstimateResponse>;
