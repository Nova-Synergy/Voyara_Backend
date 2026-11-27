using MediatR;
using Voyara.Application.CostEstimation.Services;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.CostEstimation.Queries.EstimateTransportationCost;

/// <summary>
/// Handles transportation cost estimation for a journey.
/// </summary>
public sealed class EstimateTransportationCostQueryHandler
    : IRequestHandler<EstimateTransportationCostQuery, Result<CategoryCostEstimateResponse>>
{
    private readonly IJourneyRepository _journeyRepository;
    private readonly ITransportationCostEstimator _transportationCostEstimator;

    /// <summary>
    /// Initializes a new instance of <see cref="EstimateTransportationCostQueryHandler"/>.
    /// </summary>
    public EstimateTransportationCostQueryHandler(
        IJourneyRepository journeyRepository,
        ITransportationCostEstimator transportationCostEstimator)
    {
        _journeyRepository = journeyRepository;
        _transportationCostEstimator = transportationCostEstimator;
    }

    /// <inheritdoc />
    public async Task<Result<CategoryCostEstimateResponse>> Handle(
        EstimateTransportationCostQuery request,
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
        var estimate = await _transportationCostEstimator.EstimateAsync(journey, currency, cancellationToken);

        return estimate;
    }
}
