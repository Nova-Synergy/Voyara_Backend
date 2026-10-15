using Voyara.Domain.ValueObjects;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.CostEstimation;

/// <summary>
/// Represents a snapshot calculation of estimated journey costs, containing itemized category projections and aggregate min/max totals.
/// </summary>
/// <remarks>
/// Stored as historical snapshots whenever a journey's itinerary is priced or re-estimated by the cost estimation service.
/// </remarks>
public sealed class CostEstimate : AuditableEntity
{
    private readonly List<CostLine> _lines = new();

    /// <summary>
    /// Gets the identifier of the journey itinerary for which this estimate was computed.
    /// </summary>
    /// <value>The foreign key pointing to the target journey.</value>
    public Guid JourneyId { get; private set; }

    /// <summary>
    /// Gets the aggregate minimum total estimated cost across all itemized lines.
    /// </summary>
    /// <value>The sum of all minimum cost lines in <see cref="Currency"/>.</value>
    public decimal MinTotal { get; private set; }

    /// <summary>
    /// Gets the aggregate maximum total estimated cost across all itemized lines.
    /// </summary>
    /// <value>The sum of all maximum cost lines in <see cref="Currency"/>.</value>
    public decimal MaxTotal { get; private set; }

    /// <summary>
    /// Gets the three-letter ISO 4217 currency code for the estimate totals.
    /// </summary>
    /// <value>The uppercase currency code string (e.g., "USD", "EUR").</value>
    public string Currency { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the UTC timestamp when this cost projection was computed.
    /// </summary>
    /// <value>The date and time of calculation.</value>
    public DateTime CalculatedAt { get; private set; }

    /// <summary>
    /// Gets the itemized expense lines composing this estimate.
    /// </summary>
    /// <value>A read-only collection of <see cref="CostLine"/> value objects.</value>
    public IReadOnlyCollection<CostLine> Lines => _lines.AsReadOnly();

    private CostEstimate()
    {
    }

    private CostEstimate(Guid journeyId, string currency, IEnumerable<CostLine> lines)
    {
        JourneyId = journeyId;
        Currency = currency?.Trim().ToUpperInvariant() ?? string.Empty;
        CalculatedAt = DateTime.UtcNow;

        if (lines is not null)
        {
            _lines.AddRange(lines);
        }

        RecalculateTotals();
    }

    /// <summary>
    /// Factory method to construct a new <see cref="CostEstimate"/> with itemized cost lines.
    /// </summary>
    /// <param name="journeyId">The identifier of the priced journey itinerary.</param>
    /// <param name="currency">The three-letter ISO currency code.</param>
    /// <param name="lines">The collection of category-level cost lines.</param>
    /// <returns>A new <see cref="CostEstimate"/> instance with recalculated aggregate totals.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="lines"/> is empty.</exception>
    public static CostEstimate Create(Guid journeyId, string currency, IEnumerable<CostLine> lines)
    {
        var lineList = lines?.ToList() ?? new List<CostLine>();
        if (lineList.Count == 0)
        {
            throw new ArgumentException("A cost estimate must include at least one cost line.", nameof(lines));
        }

        return new CostEstimate(journeyId, currency, lineList);
    }

    /// <summary>
    /// Adds an additional cost line to the estimate and recalculates the totals.
    /// </summary>
    /// <param name="line">The cost line to add.</param>
    public void AddLine(CostLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        _lines.Add(line);
        RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        MinTotal = _lines.Sum(l => l.Min);
        MaxTotal = _lines.Sum(l => l.Max);
    }
}

