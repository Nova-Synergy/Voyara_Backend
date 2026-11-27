using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.CostEstimation.Queries.GetJourneyCostBreakdown;

/// <summary>
/// Query to retrieve a comprehensive cost breakdown and itemized estimates for a journey.
/// </summary>
/// <param name="JourneyId">The unique identifier of the journey.</param>
/// <param name="Currency">Target ISO currency code (defaults to USD).</param>
public sealed record GetJourneyCostBreakdownQuery(
    Guid JourneyId,
    string? Currency = "USD") : IQuery<JourneyCostBreakdownResponse>;
