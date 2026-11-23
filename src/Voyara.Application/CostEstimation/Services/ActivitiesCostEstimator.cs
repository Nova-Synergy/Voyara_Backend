using Voyara.Application.DTOs;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Entities.TripAndJourney;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Catalog;
using Voyara.Domain.ValueObjects;

namespace Voyara.Application.CostEstimation.Services;

/// <summary>
/// Service calculating activities and ticket admission costs for destination stops in a journey.
/// </summary>
public sealed class ActivitiesCostEstimator : IActivitiesCostEstimator
{
    private readonly IDestinationRepository _destinationRepository;

    /// <summary>
    /// Initializes a new instance of <see cref="ActivitiesCostEstimator"/>.
    /// </summary>
    /// <param name="destinationRepository">Repository to query destination details and ticket prices.</param>
    public ActivitiesCostEstimator(IDestinationRepository destinationRepository)
    {
        _destinationRepository = destinationRepository;
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
            CostCategory.Activities.ToString(),
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
        decimal totalMin = 0m;
        decimal totalMax = 0m;

        foreach (var day in journey.Days)
        {
            foreach (var item in day.Items.OrderBy(i => i.SortOrder))
            {
                var destination = await _destinationRepository.GetByIdAsync(item.DestinationId, cancellationToken);

                if (destination is not null)
                {
                    var destName = destination.GetTranslation()?.Name ?? "Destination";

                    // Check for adult ticket or first available ticket price
                    var ticket = destination.TicketPrices.FirstOrDefault(t => t.VisitorType == VisitorType.Adult)
                                 ?? destination.TicketPrices.FirstOrDefault();

                    if (ticket is not null && ticket.Price.Amount > 0)
                    {
                        var ticketAmount = ticket.Price.Amount;
                        var money = Money.Create(ticketAmount, targetCurrency);
                        var desc = $"Day {day.DayNumber}: {destName} admission ticket ({ticket.VisitorType})";
                        costLines.Add(new CostLine(CostCategory.Tickets, desc, money));

                        totalMin += ticketAmount * 0.8m; // Discounted/concession rate
                        totalMax += ticketAmount * 1.2m; // Guided / fast-track rate
                    }
                    else
                    {
                        // Nominal sightseeing / activity fee based on duration
                        var nominalFee = destination.EstimatedVisitMinutes > 90 ? 25m : 15m;
                        var money = Money.Create(nominalFee, targetCurrency);
                        var desc = $"Day {day.DayNumber}: {destName} sightseeing & activity";
                        costLines.Add(new CostLine(CostCategory.Activities, desc, money));

                        totalMin += nominalFee * 0.7m;
                        totalMax += nominalFee * 1.4m;
                    }
                }
                else
                {
                    // Fallback nominal activity estimation when destination details are uncatalogued
                    var fallbackFee = 20m;
                    var money = Money.Create(fallbackFee, targetCurrency);
                    var desc = $"Day {day.DayNumber}: Stop #{item.SortOrder} activity & admission";
                    costLines.Add(new CostLine(CostCategory.Activities, desc, money));

                    totalMin += 10m;
                    totalMax += 35m;
                }
            }
        }

        // If journey has days but no specific items, supply a general activity baseline per day
        if (costLines.Count == 0 && journey.TotalDays > 0)
        {
            var dailyBaseline = 30m;
            for (int dayNum = 1; dayNum <= journey.TotalDays; dayNum++)
            {
                var money = Money.Create(dailyBaseline, targetCurrency);
                costLines.Add(new CostLine(CostCategory.Activities, $"Day {dayNum}: General city activities & leisure", money));
            }

            totalMin = dailyBaseline * 0.6m * journey.TotalDays;
            totalMax = dailyBaseline * 1.5m * journey.TotalDays;
        }

        var minMoney = Money.Create(Math.Max(0m, totalMin), targetCurrency);
        var maxMoney = Money.Create(Math.Max(totalMin, totalMax), targetCurrency);
        var range = CostRange.Create(minMoney, maxMoney);

        return (costLines.AsReadOnly(), range);
    }
}
