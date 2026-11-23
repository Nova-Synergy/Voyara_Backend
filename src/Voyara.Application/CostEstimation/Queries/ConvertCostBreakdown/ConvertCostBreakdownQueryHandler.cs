using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.CostEstimation.Services;
using Voyara.Application.DTOs;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.CostEstimation.Queries.ConvertCostBreakdown;

/// <summary>
/// Handles converting a journey's cost breakdown to a requested target currency using foreign exchange rates.
/// </summary>
public sealed class ConvertCostBreakdownQueryHandler
    : IRequestHandler<ConvertCostBreakdownQuery, Result<ConvertedCostBreakdownResponse>>
{
    private const string DisclaimerFormat =
        "Converted using indicative exchange rate 1 {0} = {1:F4} {2}. Provider tariffs and conversion fees may apply.";

    private readonly IJourneyRepository _journeyRepository;
    private readonly ITransportationCostEstimator _transportationEstimator;
    private readonly IAccommodationCostEstimator _accommodationEstimator;
    private readonly IActivitiesCostEstimator _activitiesEstimator;
    private readonly IExchangeRateProvider _exchangeRateProvider;

    /// <summary>
    /// Initializes a new instance of <see cref="ConvertCostBreakdownQueryHandler"/>.
    /// </summary>
    public ConvertCostBreakdownQueryHandler(
        IJourneyRepository journeyRepository,
        ITransportationCostEstimator transportationEstimator,
        IAccommodationCostEstimator accommodationEstimator,
        IActivitiesCostEstimator activitiesEstimator,
        IExchangeRateProvider exchangeRateProvider)
    {
        _journeyRepository = journeyRepository;
        _transportationEstimator = transportationEstimator;
        _accommodationEstimator = accommodationEstimator;
        _activitiesEstimator = activitiesEstimator;
        _exchangeRateProvider = exchangeRateProvider;
    }

    /// <inheritdoc />
    public async Task<Result<ConvertedCostBreakdownResponse>> Handle(
        ConvertCostBreakdownQuery request,
        CancellationToken cancellationToken)
    {
        var journey = await _journeyRepository.GetByIdAsync(request.JourneyId, cancellationToken);
        if (journey is null)
        {
            return Result.Failure<ConvertedCostBreakdownResponse>(Error.NotFound(
                "Journey.NotFound",
                $"Journey '{request.JourneyId}' was not found."));
        }

        var targetCurrency = request.TargetCurrency.Trim().ToUpperInvariant();

        // 1. Determine base lines and categories from latest snapshot or dynamic estimation
        var latestSnapshot = journey.CostSnapshots.OrderByDescending(s => s.CalculatedAt).FirstOrDefault();

        string baseCurrency;
        Guid? estimateId = null;
        List<CostLineResponse> baseLines;
        List<CategoryCostEstimateResponse> baseCategories;
        decimal baseTotal;
        decimal baseMin;
        decimal baseMax;

        if (latestSnapshot is not null)
        {
            estimateId = latestSnapshot.Id;
            baseCurrency = latestSnapshot.TotalCost.Currency;
            baseTotal = latestSnapshot.TotalCost.Amount;
            baseMin = latestSnapshot.EstimatedRange?.Minimum.Amount ?? (baseTotal * 0.85m);
            baseMax = latestSnapshot.EstimatedRange?.Maximum.Amount ?? (baseTotal * 1.25m);

            baseLines = latestSnapshot.Lines
                .Select(l => new CostLineResponse(l.Description, l.Amount.Amount, l.Amount.Currency, l.Category.ToString()))
                .ToList();

            baseCategories = latestSnapshot.Lines
                .GroupBy(l => l.Category)
                .Select(g => new CategoryCostEstimateResponse(
                    g.Key.ToString(),
                    g.Sum(l => l.Amount.Amount),
                    g.Sum(l => l.Amount.Amount) * 0.85m,
                    g.Sum(l => l.Amount.Amount) * 1.25m,
                    baseCurrency,
                    g.Select(l => new CostLineResponse(l.Description, l.Amount.Amount, l.Amount.Currency, l.Category.ToString())).ToList()))
                .ToList();
        }
        else
        {
            baseCurrency = "USD";
            var (tLines, tRange) = await _transportationEstimator.CalculateDomainLinesAsync(journey, baseCurrency, cancellationToken);
            var (aLines, aRange) = await _accommodationEstimator.CalculateDomainLinesAsync(journey, baseCurrency, tier: "Standard", cancellationToken);
            var (actLines, actRange) = await _activitiesEstimator.CalculateDomainLinesAsync(journey, baseCurrency, cancellationToken);

            baseCategories = new List<CategoryCostEstimateResponse>
            {
                new(
                    CostCategory.Transportation.ToString(),
                    tLines.Sum(l => l.Amount.Amount),
                    tRange.Minimum.Amount,
                    tRange.Maximum.Amount,
                    baseCurrency,
                    tLines.Select(l => new CostLineResponse(l.Description, l.Amount.Amount, l.Amount.Currency, l.Category.ToString())).ToList()),

                new(
                    CostCategory.Accommodation.ToString(),
                    aLines.Sum(l => l.Amount.Amount),
                    aRange.Minimum.Amount,
                    aRange.Maximum.Amount,
                    baseCurrency,
                    aLines.Select(l => new CostLineResponse(l.Description, l.Amount.Amount, l.Amount.Currency, l.Category.ToString())).ToList()),

                new(
                    CostCategory.Activities.ToString(),
                    actLines.Sum(l => l.Amount.Amount),
                    actRange.Minimum.Amount,
                    actRange.Maximum.Amount,
                    baseCurrency,
                    actLines.Select(l => new CostLineResponse(l.Description, l.Amount.Amount, l.Amount.Currency, l.Category.ToString())).ToList())
            };

            baseLines = tLines.Concat(aLines).Concat(actLines)
                .Select(l => new CostLineResponse(l.Description, l.Amount.Amount, l.Amount.Currency, l.Category.ToString()))
                .ToList();

            baseTotal = baseCategories.Sum(c => c.TotalAmount);
            baseMin = tRange.Minimum.Amount + aRange.Minimum.Amount + actRange.Minimum.Amount;
            baseMax = tRange.Maximum.Amount + aRange.Maximum.Amount + actRange.Maximum.Amount;
        }

        // 2. Fetch exchange rate
        var rateResult = await _exchangeRateProvider.GetRateAsync(baseCurrency, targetCurrency, cancellationToken);
        if (rateResult.IsFailure)
        {
            return Result.Failure<ConvertedCostBreakdownResponse>(rateResult.Error);
        }

        var rate = rateResult.Value;

        // 3. Convert all figures
        var convertedLines = baseLines
            .Select(l => new CostLineResponse(
                l.Description,
                Math.Round(l.Amount * rate, 2),
                targetCurrency,
                l.Category))
            .ToList();

        var convertedCategories = baseCategories
            .Select(c => new CategoryCostEstimateResponse(
                c.Category,
                Math.Round(c.TotalAmount * rate, 2),
                Math.Round(c.MinAmount * rate, 2),
                Math.Round(c.MaxAmount * rate, 2),
                targetCurrency,
                c.Lines.Select(l => new CostLineResponse(
                    l.Description,
                    Math.Round(l.Amount * rate, 2),
                    targetCurrency,
                    l.Category)).ToList()))
            .ToList();

        var convertedTotal = Math.Round(baseTotal * rate, 2);
        var convertedMin = Math.Round(baseMin * rate, 2);
        var convertedMax = Math.Round(baseMax * rate, 2);

        var disclaimer = string.Format(DisclaimerFormat, baseCurrency, rate, targetCurrency);

        return new ConvertedCostBreakdownResponse(
            journey.Id,
            estimateId,
            baseCurrency,
            targetCurrency,
            rate,
            convertedTotal,
            convertedMin,
            convertedMax,
            convertedCategories,
            convertedLines,
            disclaimer);
    }
}
