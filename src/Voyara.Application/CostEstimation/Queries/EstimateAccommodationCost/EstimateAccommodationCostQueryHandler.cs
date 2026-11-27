using MediatR;
using Voyara.Application.CostEstimation.Services;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.CostEstimation.Queries.EstimateAccommodationCost;

/// <summary>
/// Handles accommodation cost estimation for a journey.
/// </summary>
public sealed class EstimateAccommodationCostQueryHandler
    : IRequestHandler<EstimateAccommodationCostQuery, Result<CategoryCostEstimateResponse>>
{
    private readonly IJourneyRepository _journeyRepository;
    private readonly IAccommodationCostEstimator _accommodationCostEstimator;

    /// <summary>
    /// Initializes a new instance of <see cref="EstimateAccommodationCostQueryHandler"/>.
    /// </summary>
    public EstimateAccommodationCostQueryHandler(
        IJourneyRepository journeyRepository,
        IAccommodationCostEstimator accommodationCostEstimator)
    {
        _journeyRepository = journeyRepository;
        _accommodationCostEstimator = accommodationCostEstimator;
    }

    /// <inheritdoc />
    public async Task<Result<CategoryCostEstimateResponse>> Handle(
        EstimateAccommodationCostQuery request,
        CancellationToken cancellationToken)
    {
        var journey = await _journeyRepository.GetByIdAsync(request.JourneyId, cancellationToken);
        if (journey is null)
        {
            return Result.Failure<CategoryCostEstimateResponse>(Error.NotFound(
                "Journey.NotFound",
                $"Journey '{request.JourneyId}' was not found."));
        }

        var currency = string.IsNullOrWhiteSpace(request.Currency) ? "USD" : request.Currency;
        var estimate = await _accommodationCostEstimator.EstimateAsync(
            journey,
            currency,
            request.Tier,
            cancellationToken);

        return estimate;
    }
}
