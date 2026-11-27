using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.CostEstimation.Queries.EstimateActivitiesCost;

/// <summary>
/// Query to calculate activities and tickets cost estimates for a journey itinerary.
/// </summary>
/// <param name="JourneyId">The unique identifier of the journey.</param>
/// <param name="Currency">Target ISO currency code (defaults to USD).</param>
public sealed record EstimateActivitiesCostQuery(
    Guid JourneyId,
    string? Currency = "USD") : IQuery<CategoryCostEstimateResponse>;
