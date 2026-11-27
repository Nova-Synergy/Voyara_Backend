using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.CostEstimation.Queries.ConvertCostBreakdown;

/// <summary>
/// Query to convert a journey's itemized cost breakdown into a target currency.
/// </summary>
/// <param name="JourneyId">The unique identifier of the journey.</param>
/// <param name="TargetCurrency">Target ISO currency code (e.g. EUR, GBP, USD).</param>
public sealed record ConvertCostBreakdownQuery(
    Guid JourneyId,
    string TargetCurrency) : IQuery<ConvertedCostBreakdownResponse>;
