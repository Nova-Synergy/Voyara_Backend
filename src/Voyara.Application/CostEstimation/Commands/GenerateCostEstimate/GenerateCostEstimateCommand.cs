using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.CostEstimation.Commands.GenerateCostEstimate;

/// <summary>
/// Command to calculate and persist a comprehensive cost estimate snapshot for a journey itinerary.
/// </summary>
/// <param name="JourneyId">The unique identifier of the journey.</param>
/// <param name="Currency">Target ISO currency code (defaults to USD).</param>
/// <param name="LodgingTier">Optional lodging tier preference (e.g. Budget, Standard, Luxury).</param>
public sealed record GenerateCostEstimateCommand(
    Guid JourneyId,
    string? Currency = "USD",
    string? LodgingTier = "Standard") : ICommand<CostEstimateResultResponse>;
