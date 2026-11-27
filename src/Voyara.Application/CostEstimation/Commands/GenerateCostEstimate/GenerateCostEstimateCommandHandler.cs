using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.CostEstimation.Services;
using Voyara.Application.DTOs;
using Voyara.Domain.Entities.CostEstimation;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.Domain.ValueObjects;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.CostEstimation.Commands.GenerateCostEstimate;

/// <summary>
/// Handles orchestrating category cost estimators, creating a domain CostEstimate snapshot,
/// recording it on the journey aggregate, and persisting changes.
/// </summary>
public sealed class GenerateCostEstimateCommandHandler
    : IRequestHandler<GenerateCostEstimateCommand, Result<CostEstimateResultResponse>>
{
    private readonly IJourneyRepository _journeyRepository;
    private readonly ITransportationCostEstimator _transportationEstimator;
    private readonly IAccommodationCostEstimator _accommodationEstimator;
    private readonly IActivitiesCostEstimator _activitiesEstimator;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of <see cref="GenerateCostEstimateCommandHandler"/>.
    /// </summary>
    public GenerateCostEstimateCommandHandler(
        IJourneyRepository journeyRepository,
        ITransportationCostEstimator transportationEstimator,
        IAccommodationCostEstimator accommodationEstimator,
        IActivitiesCostEstimator activitiesEstimator,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _journeyRepository = journeyRepository;
        _transportationEstimator = transportationEstimator;
        _accommodationEstimator = accommodationEstimator;
        _activitiesEstimator = activitiesEstimator;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<CostEstimateResultResponse>> Handle(
        GenerateCostEstimateCommand request,
        CancellationToken cancellationToken)
    {
        var journey = await _journeyRepository.GetByIdAsync(request.JourneyId, cancellationToken);
        if (journey is null)
        {
            return Result.Failure<CostEstimateResultResponse>(Error.NotFound(
                "Journey.NotFound",
                $"Journey '{request.JourneyId}' was not found."));
        }

        var currency = string.IsNullOrWhiteSpace(request.Currency) ? "USD" : request.Currency.Trim().ToUpperInvariant();
        var userId = _currentUser.UserId ?? Guid.NewGuid();
        var tripId = journey.TripId ?? journey.Id;

        // Calculate domain lines and ranges from each specialized estimator
        var (transportLines, transportRange) = await _transportationEstimator.CalculateDomainLinesAsync(
            journey,
            currency,
            cancellationToken);

        var (accommodationLines, accommodationRange) = await _accommodationEstimator.CalculateDomainLinesAsync(
            journey,
            currency,
            request.LodgingTier,
            cancellationToken);

        var (activitiesLines, activitiesRange) = await _activitiesEstimator.CalculateDomainLinesAsync(
            journey,
            currency,
            cancellationToken);

        var allLines = new List<CostLine>();
        allLines.AddRange(transportLines);
        allLines.AddRange(accommodationLines);
        allLines.AddRange(activitiesLines);

        if (allLines.Count == 0)
        {
            allLines.Add(new CostLine(CostCategory.Miscellaneous, "General travel planning allowance", Money.Create(50m, currency)));
        }

        var totalMin = transportRange.Minimum.Amount + accommodationRange.Minimum.Amount + activitiesRange.Minimum.Amount;
        var totalMax = transportRange.Maximum.Amount + accommodationRange.Maximum.Amount + activitiesRange.Maximum.Amount;
        var totalRange = CostRange.Create(
            Money.Create(totalMin, currency),
            Money.Create(Math.Max(totalMin, totalMax), currency));

        var costEstimateResult = CostEstimate.Create(tripId, userId, allLines, totalRange);
        if (costEstimateResult.IsFailure)
        {
            return Result.Failure<CostEstimateResultResponse>(costEstimateResult.Error);
        }

        var costEstimate = costEstimateResult.Value;
        journey.RecordCostSnapshot(costEstimate);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map category responses
        var categories = new List<CategoryCostEstimateResponse>
        {
            new(
                CostCategory.Transportation.ToString(),
                transportLines.Sum(l => l.Amount.Amount),
                transportRange.Minimum.Amount,
                transportRange.Maximum.Amount,
                currency,
                transportLines.Select(l => new CostLineResponse(l.Description, l.Amount.Amount, l.Amount.Currency, l.Category.ToString())).ToList()),

            new(
                CostCategory.Accommodation.ToString(),
                accommodationLines.Sum(l => l.Amount.Amount),
                accommodationRange.Minimum.Amount,
                accommodationRange.Maximum.Amount,
                currency,
                accommodationLines.Select(l => new CostLineResponse(l.Description, l.Amount.Amount, l.Amount.Currency, l.Category.ToString())).ToList()),

            new(
                CostCategory.Activities.ToString(),
                activitiesLines.Sum(l => l.Amount.Amount),
                activitiesRange.Minimum.Amount,
                activitiesRange.Maximum.Amount,
                currency,
                activitiesLines.Select(l => new CostLineResponse(l.Description, l.Amount.Amount, l.Amount.Currency, l.Category.ToString())).ToList())
        };

        var allLineResponses = allLines
            .Select(l => new CostLineResponse(l.Description, l.Amount.Amount, l.Amount.Currency, l.Category.ToString()))
            .ToList();

        return new CostEstimateResultResponse(
            costEstimate.Id,
            journey.Id,
            journey.TripId,
            userId,
            costEstimate.CalculatedAt,
            costEstimate.TotalCost.Amount,
            totalMin,
            totalMax,
            currency,
            categories,
            allLineResponses);
    }
}
