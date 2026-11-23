using Voyara.Application.DTOs;
using Voyara.Domain.Entities.TripAndJourney;
using Voyara.Domain.Enums;
using Voyara.Domain.ValueObjects;

namespace Voyara.Application.CostEstimation.Services;

/// <summary>
/// Service calculating accommodation cost estimates based on itinerary nights, lodging tiers, and market rates.
/// </summary>
public sealed class AccommodationCostEstimator : IAccommodationCostEstimator
{
    /// <inheritdoc />
    public async Task<CategoryCostEstimateResponse> EstimateAsync(
        Journey journey,
        string currency = "USD",
        string? tier = null,
        CancellationToken cancellationToken = default)
    {
        var (domainLines, range) = await CalculateDomainLinesAsync(journey, currency, tier, cancellationToken);

        var lines = domainLines
            .Select(l => new CostLineResponse(
                l.Description,
                l.Amount.Amount,
                l.Amount.Currency,
                l.Category.ToString()))
            .ToList();

        var total = domainLines.Sum(l => l.Amount.Amount);

        return new CategoryCostEstimateResponse(
            CostCategory.Accommodation.ToString(),
            total,
            range.Minimum.Amount,
            range.Maximum.Amount,
            currency.ToUpperInvariant(),
            lines);
    }

    /// <inheritdoc />
    public Task<(IReadOnlyList<CostLine> Lines, CostRange Range)> CalculateDomainLinesAsync(
        Journey journey,
        string currency = "USD",
        string? tier = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(journey);
        var targetCurrency = string.IsNullOrWhiteSpace(currency) ? "USD" : currency.Trim().ToUpperInvariant();

        // Calculate nights: a trip of N days typically entails (N - 1) nights, minimum 1 night if duration >= 1
        var nights = Math.Max(1, journey.TotalDays > 1 ? journey.TotalDays - 1 : 1);

        var (nominalRate, minRate, maxRate, tierLabel) = ResolveTierRates(tier);

        var costLines = new List<CostLine>();

        for (int night = 1; night <= nights; night++)
        {
            var money = Money.Create(nominalRate, targetCurrency);
            var description = $"Night {night}: {tierLabel} lodging estimate";
            costLines.Add(new CostLine(CostCategory.Accommodation, description, money));
        }

        var totalMin = minRate * nights;
        var totalMax = maxRate * nights;

        var minMoney = Money.Create(totalMin, targetCurrency);
        var maxMoney = Money.Create(Math.Max(totalMin, totalMax), targetCurrency);
        var range = CostRange.Create(minMoney, maxMoney);

        return Task.FromResult(((IReadOnlyList<CostLine>)costLines.AsReadOnly(), range));
    }

    private static (decimal Nominal, decimal Min, decimal Max, string Label) ResolveTierRates(string? tier)
    {
        if (string.IsNullOrWhiteSpace(tier))
        {
            return (80m, 50m, 120m, "Standard hotel (3-Star)");
        }

        var normalized = tier.Trim().ToLowerInvariant();
        if (normalized.Contains("budget") || normalized.Contains("hostel") || normalized.Contains("economy"))
        {
            return (35m, 20m, 50m, "Budget / hostel stay");
        }

        if (normalized.Contains("lux") || normalized.Contains("premium") || normalized.Contains("5-star"))
        {
            return (220m, 150m, 350m, "Luxury resort / 5-Star stay");
        }

        return (80m, 50m, 120m, "Standard hotel (3-Star)");
    }
}
