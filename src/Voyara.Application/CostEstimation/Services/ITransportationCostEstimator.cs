using Voyara.Application.DTOs;
using Voyara.Domain.Entities.TripAndJourney;
using Voyara.Domain.ValueObjects;

namespace Voyara.Application.CostEstimation.Services;

/// <summary>
/// Domain service contract for estimating transportation expenses across a journey itinerary.
/// </summary>
public interface ITransportationCostEstimator
{
    /// <summary>
    /// Calculates itemized cost lines and an estimated range for all transportation within a journey.
    /// </summary>
    /// <param name="journey">The journey aggregate root.</param>
    /// <param name="currency">The target ISO currency code (defaults to USD).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="CategoryCostEstimateResponse"/> with itemized lines and estimated range.</returns>
    Task<CategoryCostEstimateResponse> EstimateAsync(
        Journey journey,
        string currency = "USD",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates domain <see cref="CostLine"/> value objects and a <see cref="CostRange"/> for transportation.
    /// </summary>
    /// <param name="journey">The journey aggregate root.</param>
    /// <param name="currency">Target ISO currency code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple of domain cost lines and the estimated cost range.</returns>
    Task<(IReadOnlyList<CostLine> Lines, CostRange Range)> CalculateDomainLinesAsync(
        Journey journey,
        string currency = "USD",
        CancellationToken cancellationToken = default);
}
