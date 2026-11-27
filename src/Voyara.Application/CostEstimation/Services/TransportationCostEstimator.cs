using Voyara.Application.DTOs;
using Voyara.Domain.Entities.TripAndJourney;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Transportation;
using Voyara.Domain.ValueObjects;

namespace Voyara.Application.CostEstimation.Services;

/// <summary>
/// Service calculating transportation cost estimates based on itinerary transit legs, modes, and city transport rates.
/// </summary>
public sealed class TransportationCostEstimator : ITransportationCostEstimator
{
    private readonly ICityTransportRepository _cityTransportRepository;

    /// <summary>
    /// Initializes a new instance of <see cref="TransportationCostEstimator"/>.
    /// </summary>
    public TransportationCostEstimator(ICityTransportRepository cityTransportRepository)
    {
        _cityTransportRepository = cityTransportRepository;
    }

    /// <inheritdoc />
    public async Task<CategoryCostEstimateResponse> EstimateAsync(
        Journey journey,
        string currency = "USD",
        CancellationToken cancellationToken = default)
    {
        var (domainLines, range) = await CalculateDomainLinesAsync(journey, currency, cancellationToken);

        var lines = domainLines
            .Select(l => new CostLineResponse(
                l.Description,
                l.Amount.Amount,
                l.Amount.Currency,
                l.Category.ToString()))
            .ToList();

        var total = domainLines.Sum(l => l.Amount.Amount);

        return new CategoryCostEstimateResponse(
            CostCategory.Transportation.ToString(),
            total,
            range.Minimum.Amount,
            range.Maximum.Amount,
            currency.ToUpperInvariant(),
            lines);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<CostLine> Lines, CostRange Range)> CalculateDomainLinesAsync(
        Journey journey,
        string currency = "USD",
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(journey);
        var targetCurrency = string.IsNullOrWhiteSpace(currency) ? "USD" : currency.Trim().ToUpperInvariant();

        var costLines = new List<CostLine>();
        decimal minTotal = 0;
        decimal maxTotal = 0;

        foreach (var day in journey.Days.OrderBy(d => d.DayNumber))
        {
            if (day.Items.Count == 0)
            {
                continue;
            }

            foreach (var item in day.Items.OrderBy(i => i.SortOrder))
            {
                var (cost, minCost, maxCost, desc) = GetEstimatedModeCost(item.Transport, day.DayNumber);

                var money = Money.Create(cost, targetCurrency);
                costLines.Add(new CostLine(CostCategory.Transportation, desc, money));

                minTotal += minCost;
                maxTotal += maxCost;
            }
        }

        // If journey has no stops, provide a nominal base estimate
        if (costLines.Count == 0)
        {
            var baseMoney = Money.Create(0m, targetCurrency);
            costLines.Add(new CostLine(
                CostCategory.Transportation,
                $"General transit estimate ({journey.TotalDays} days)",
                baseMoney));
            minTotal = 0m;
            maxTotal = 0m;
        }

        var minMoney = Money.Create(minTotal, targetCurrency);
        var maxMoney = Money.Create(Math.Max(minTotal, maxTotal), targetCurrency);
        var range = CostRange.Create(minMoney, maxMoney);

        return (costLines.AsReadOnly(), range);
    }

    private static (decimal Nominal, decimal Min, decimal Max, string Description) GetEstimatedModeCost(
        TransportMode mode,
        int dayNumber)
    {
        return mode switch
        {
            TransportMode.Walking =>
                (0m, 0m, 0m, $"Day {dayNumber}: Walking transfer (Free)"),

            TransportMode.Cycling =>
                (10m, 5m, 15m, $"Day {dayNumber}: Micro-mobility / bicycle rental"),

            TransportMode.PublicTransport =>
                (5m, 3m, 8m, $"Day {dayNumber}: Public transit (metro/bus fare)"),

            TransportMode.Driving =>
                (25m, 15m, 40m, $"Day {dayNumber}: Taxi / ride-hailing / rental car transit"),

            TransportMode.Boat =>
                (30m, 20m, 50m, $"Day {dayNumber}: Ferry / boat watercraft transfer"),

            TransportMode.Flight =>
                (120m, 80m, 180m, $"Day {dayNumber}: Regional / domestic flight"),

            _ => (10m, 5m, 20m, $"Day {dayNumber}: Ground transit transfer")
        };
    }
}
