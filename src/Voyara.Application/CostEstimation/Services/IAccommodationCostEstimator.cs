using Voyara.Application.DTOs;
using Voyara.Domain.Entities.TripAndJourney;
using Voyara.Domain.ValueObjects;

namespace Voyara.Application.CostEstimation.Services;

/// <summary>
/// Domain service contract for estimating lodging and accommodation expenses for a journey.
/// </summary>
public interface IAccommodationCostEstimator
{
    /// <summary>
    /// Calculates itemized cost lines and an estimated range for lodging across a journey.
    /// </summary>
    /// <param name="journey">The journey aggregate root.</param>
    /// <param name="currency">The target ISO currency code (defaults to USD).</param>
    /// <param name="tier">Optional lodging tier (e.g. "Budget", "Standard", "Luxury").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="CategoryCostEstimateResponse"/> with itemized lines and estimated range.</returns>
    Task<CategoryCostEstimateResponse> EstimateAsync(
        Journey journey,
        string currency = "USD",
        string? tier = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates domain <see cref="CostLine"/> value objects and a <see cref="CostRange"/> for accommodation.
    /// </summary>
    /// <param name="journey">The journey aggregate root.</param>
    /// <param name="currency">Target ISO currency code.</param>
    /// <param name="tier">Optional lodging tier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple of domain cost lines and the estimated cost range.</returns>
    Task<(IReadOnlyList<CostLine> Lines, CostRange Range)> CalculateDomainLinesAsync(
        Journey journey,
        string currency = "USD",
        string? tier = null,
        CancellationToken cancellationToken = default);
}
