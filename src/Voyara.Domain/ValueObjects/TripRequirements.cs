using Voyara.SharedKernel.ValueObjects;

namespace Voyara.Domain.ValueObjects;

/// <summary>
/// Value object capturing user-specified requirements and preferences collected during an AI planning session.
/// </summary>
/// <remarks>
/// Encapsulates destination country, duration, budgetary parameters, and travel style preferences.
/// Being a value object, it is immutable and compared by value.
/// </remarks>
public sealed class TripRequirements : ValueObject
{
    /// <summary>
    /// Gets the target destination country or region.
    /// </summary>
    /// <value>The destination country name, or an empty string if not yet specified.</value>
    public string Country { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the planned duration of the trip in days.
    /// </summary>
    /// <value>The number of days planned for the journey.</value>
    public int DurationDays { get; private set; }

    /// <summary>
    /// Gets the tourist interests and activity preferences (e.g., historical, culinary, outdoors).
    /// </summary>
    /// <value>A read-only collection of interest keywords.</value>
    public IReadOnlyList<string> Interests { get; private set; } = Array.Empty<string>();

    /// <summary>
    /// Gets the designated overall budget amount.
    /// </summary>
    /// <value>The decimal budget ceiling or target amount.</value>
    public decimal BudgetAmount { get; private set; }

    /// <summary>
    /// Gets the three-letter ISO 4217 currency code for the budget.
    /// </summary>
    /// <value>The uppercase currency code (e.g., "USD", "EUR").</value>
    public string BudgetCurrency { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the preferred style of travel (e.g., "Relaxed", "Adventure", "Cultural", "Luxury").
    /// </summary>
    /// <value>A descriptive string representing the user's travel tempo and style.</value>
    public string TravelStyle { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the preferred mode of transportation during travel (e.g., "PublicTransport", "RentalCar").
    /// </summary>
    /// <value>A descriptive string of transport preference.</value>
    public string TransportPreference { get; private set; } = string.Empty;

    private TripRequirements()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TripRequirements"/> class with specified travel preferences.
    /// </summary>
    /// <param name="country">Target destination country.</param>
    /// <param name="durationDays">Total duration in days.</param>
    /// <param name="interests">Collection of tourist interest tags.</param>
    /// <param name="budgetAmount">Planned total spending budget.</param>
    /// <param name="budgetCurrency">ISO 4217 currency code for the budget amount.</param>
    /// <param name="travelStyle">Travel style and pacing preference.</param>
    /// <param name="transportPreference">Preferred transportation type.</param>
    public TripRequirements(
        string country,
        int durationDays,
        IEnumerable<string>? interests,
        decimal budgetAmount,
        string budgetCurrency,
        string travelStyle,
        string transportPreference)
    {
        Country = country?.Trim() ?? string.Empty;
        DurationDays = Math.Max(0, durationDays);
        Interests = interests?.Where(i => !string.IsNullOrWhiteSpace(i)).Select(i => i.Trim()).ToList().AsReadOnly()
                    ?? (IReadOnlyList<string>)Array.Empty<string>();
        BudgetAmount = Math.Max(0, budgetAmount);
        BudgetCurrency = budgetCurrency?.Trim().ToUpperInvariant() ?? string.Empty;
        TravelStyle = travelStyle?.Trim() ?? string.Empty;
        TransportPreference = transportPreference?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Determines whether all essential travel requirement fields have been populated.
    /// </summary>
    /// <returns><see langword="true"/> if all required attributes are non-empty and positive; otherwise, <see langword="false"/>.</returns>
    public bool IsComplete() => MissingFields().Count == 0;

    /// <summary>
    /// Identifies and returns the names of all missing or incomplete required requirement fields.
    /// </summary>
    /// <returns>A list containing the property names of incomplete travel requirements.</returns>
    public List<string> MissingFields()
    {
        var missing = new List<string>();

        if (string.IsNullOrWhiteSpace(Country))
        {
            missing.Add(nameof(Country));
        }

        if (DurationDays <= 0)
        {
            missing.Add(nameof(DurationDays));
        }

        if (BudgetAmount <= 0)
        {
            missing.Add(nameof(BudgetAmount));
        }

        if (string.IsNullOrWhiteSpace(BudgetCurrency))
        {
            missing.Add(nameof(BudgetCurrency));
        }

        if (string.IsNullOrWhiteSpace(TravelStyle))
        {
            missing.Add(nameof(TravelStyle));
        }

        return missing;
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Country;
        yield return DurationDays;
        yield return BudgetAmount;
        yield return BudgetCurrency;
        yield return TravelStyle;
        yield return TransportPreference;
        foreach (var interest in Interests)
        {
            yield return interest;
        }
    }
}

