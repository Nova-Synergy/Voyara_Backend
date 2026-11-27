using MediatR;
using Voyara.Application.CostEstimation.Services;
using Voyara.Application.DTOs;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.CostEstimation.Queries.GetJourneyCostBreakdown;

/// <summary>
/// Handles retrieving the latest cost breakdown for a journey, falling back to a dynamic
/// on-demand estimation if no snapshot has been persisted yet.
/// </summary>
public sealed class GetJourneyCostBreakdownQueryHandler
    : IRequestHandler<GetJourneyCostBreakdownQuery, Result<JourneyCostBreakdownResponse>>
{
    private const string DefaultDisclaimer =
        "Estimates are calculated using current market averages and supplier tariffs. Actual on-ground costs may fluctuate.";

    private readonly IJourneyRepository _journeyRepository;
    private readonly ITransportationCostEstimator _transportationEstimator;
    private readonly IAccommodationCostEstimator _accommodationEstimator;
    private readonly IActivitiesCostEstimator _activitiesEstimator;

    /// <summary>
    /// Initializes a new instance of <see cref="GetJourneyCostBreakdownQueryHandler"/>.
    /// </summary>
    public GetJourneyCostBreakdownQueryHandler(
        IJourneyRepository journeyRepository,
        ITransportationCostEstimator transportationEstimator,
        IAccommodationCostEstimator accommodationEstimator,
        IActivitiesCostEstimator activitiesEstimator)
    {
        _journeyRepository = journeyRepository;
        _transportationEstimator = transportationEstimator;
        _accommodationEstimator = accommodationEstimator;
        _activitiesEstimator = activitiesEstimator;
    }

    /// <inheritdoc />
    public async Task<Result<JourneyCostBreakdownResponse>> Handle(
        GetJourneyCostBreakdownQuery request,
        CancellationToken cancellationToken)
    {
        var journey = await _journeyRepository.GetByIdAsync(request.JourneyId, cancellationToken);
        if (journey is null)
        {
            return Result.Failure<JourneyCostBreakdownResponse>(Error.NotFound(
                "Journey.NotFound",
                $"Journey '{request.JourneyId}' was not found."));
        }

        var targetCurrency = string.IsNullOrWhiteSpace(request.Currency) ? "USD" : request.Currency.Trim().ToUpperInvariant();

        // 1. Check if a persisted cost estimate snapshot exists on the journey
        var latestSnapshot = journey.CostSnapshots.OrderByDescending(s => s.CalculatedAt).FirstOrDefault();
        if (latestSnapshot is not null)
        {
            var snapshotCurrency = latestSnapshot.TotalCost.Currency;

            var snapshotCategories = latestSnapshot.Lines
                .GroupBy(l => l.Category)
                .Select(g => new CategoryCostEstimateResponse(
                    g.Key.ToString(),
                    g.Sum(l => l.Amount.Amount),
                    g.Sum(l => l.Amount.Amount) * 0.85m,
                    g.Sum(l => l.Amount.Amount) * 1.25m,
                    snapshotCurrency,
                    g.Select(l => new CostLineResponse(l.Description, l.Amount.Amount, l.Amount.Currency, l.Category.ToString())).ToList()))
                .ToList();

            var snapshotLines = latestSnapshot.Lines
                .Select(l => new CostLineResponse(l.Description, l.Amount.Amount, l.Amount.Currency, l.Category.ToString()))
                .ToList();

            var minCost = latestSnapshot.EstimatedRange?.Minimum.Amount ?? (latestSnapshot.TotalCost.Amount * 0.85m);
            var maxCost = latestSnapshot.EstimatedRange?.Maximum.Amount ?? (latestSnapshot.TotalCost.Amount * 1.25m);

            return new JourneyCostBreakdownResponse(
                journey.Id,
                latestSnapshot.Id,
                latestSnapshot.CalculatedAt,
                latestSnapshot.TotalCost.Amount,
                minCost,
                maxCost,
                snapshotCurrency,
                snapshotCategories,
                snapshotLines,
                DefaultDisclaimer);
        }

        // 2. Otherwise dynamically compute an on-demand cost breakdown
        var (transportLines, transportRange) = await _transportationEstimator.CalculateDomainLinesAsync(
            journey,
            targetCurrency,
            cancellationToken);

        var (accommodationLines, accommodationRange) = await _accommodationEstimator.CalculateDomainLinesAsync(
            journey,
            targetCurrency,
            tier: "Standard",
            cancellationToken);

        var (activitiesLines, activitiesRange) = await _activitiesEstimator.CalculateDomainLinesAsync(
            journey,
            targetCurrency,
            cancellationToken);

        var categories = new List<CategoryCostEstimateResponse>
        {
            new(
                CostCategory.Transportation.ToString(),
                transportLines.Sum(l => l.Amount.Amount),
                transportRange.Minimum.Amount,
                transportRange.Maximum.Amount,
                targetCurrency,
                transportLines.Select(l => new CostLineResponse(l.Description, l.Amount.Amount, l.Amount.Currency, l.Category.ToString())).ToList()),

            new(
                CostCategory.Accommodation.ToString(),
                accommodationLines.Sum(l => l.Amount.Amount),
                accommodationRange.Minimum.Amount,
                accommodationRange.Maximum.Amount,
                targetCurrency,
                accommodationLines.Select(l => new CostLineResponse(l.Description, l.Amount.Amount, l.Amount.Currency, l.Category.ToString())).ToList()),

            new(
                CostCategory.Activities.ToString(),
                activitiesLines.Sum(l => l.Amount.Amount),
                activitiesRange.Minimum.Amount,
                activitiesRange.Maximum.Amount,
                targetCurrency,
                activitiesLines.Select(l => new CostLineResponse(l.Description, l.Amount.Amount, l.Amount.Currency, l.Category.ToString())).ToList())
        };

        var allLines = transportLines
            .Concat(accommodationLines)
            .Concat(activitiesLines)
            .Select(l => new CostLineResponse(l.Description, l.Amount.Amount, l.Amount.Currency, l.Category.ToString()))
            .ToList();

        var totalAmount = categories.Sum(c => c.TotalAmount);
        var minTotal = transportRange.Minimum.Amount + accommodationRange.Minimum.Amount + activitiesRange.Minimum.Amount;
        var maxTotal = transportRange.Maximum.Amount + accommodationRange.Maximum.Amount + activitiesRange.Maximum.Amount;

        return new JourneyCostBreakdownResponse(
            journey.Id,
            EstimateId: null,
            CalculatedAt: DateTimeOffset.UtcNow,
            totalAmount,
            minTotal,
            maxTotal,
            targetCurrency,
            categories,
            allLines,
            DefaultDisclaimer);
    }
}
