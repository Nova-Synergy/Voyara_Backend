using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.CostEstimation.Queries.EstimateTransportationCost;

/// <summary>
/// Query to calculate transportation cost estimates for a journey itinerary.
/// </summary>
/// <param name="JourneyId">The unique identifier of the journey.</param>
/// <param name="Currency">Target ISO currency code (defaults to USD).</param>
public sealed record EstimateTransportationCostQuery(
    Guid JourneyId,
    string? Currency = "USD") : IQuery<CategoryCostEstimateResponse>;
