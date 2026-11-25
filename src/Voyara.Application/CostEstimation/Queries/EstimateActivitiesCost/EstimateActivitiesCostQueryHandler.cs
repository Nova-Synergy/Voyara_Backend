using MediatR;
using Voyara.Application.CostEstimation.Services;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.CostEstimation.Queries.EstimateActivitiesCost;

/// <summary>
/// Handles activities and tickets cost estimation for a journey.
/// </summary>
public sealed class EstimateActivitiesCostQueryHandler
    : IRequestHandler<EstimateActivitiesCostQuery, Result<CategoryCostEstimateResponse>>
{
    private readonly IJourneyRepository _journeyRepository;
    private readonly IActivitiesCostEstimator _activitiesCostEstimator;

    /// <summary>
    /// Initializes a new instance of <see cref="EstimateActivitiesCostQueryHandler"/>.
    /// </summary>
    public EstimateActivitiesCostQueryHandler(
        IJourneyRepository journeyRepository,
        IActivitiesCostEstimator activitiesCostEstimator)
    {
        _journeyRepository = journeyRepository;
        _activitiesCostEstimator = activitiesCostEstimator;
    }

    /// <inheritdoc />
    public async Task<Result<CategoryCostEstimateResponse>> Handle(
        EstimateActivitiesCostQuery request,
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
        var estimate = await _activitiesCostEstimator.EstimateAsync(journey, currency, cancellationToken);

        return estimate;
    }
}
