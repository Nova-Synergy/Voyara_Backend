using Voyara.Domain.Entities.LocalInfo;
using Voyara.Domain.Enums;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.CostEstimation;

/// <summary>
/// Represents a baseline daily expenditure rule used to compute itemized cost estimates for a destination city, budget tier, and cost category.
/// </summary>
/// <remarks>
/// Acts as the actuarial unit cost catalog referenced by the cost estimation engine when pricing journeys.
/// </remarks>
public sealed class CostRule : AuditableEntity
{
    /// <summary>
    /// Gets the database-generated integer identifier of the cost rule.
    /// </summary>
    /// <value>The unique integer key.</value>
    public new int Id { get; private set; }

    /// <summary>
    /// Gets the identifier of the city where this daily rate applies.
    /// </summary>
    /// <value>The foreign key pointing to the target <see cref="City"/>.</value>
    public int CityId { get; private set; }

    /// <summary>
    /// Gets the navigation reference to the associated city.
    /// </summary>
    /// <value>The <see cref="City"/> entity.</value>
    public City? City { get; private set; }

    /// <summary>
    /// Gets the spending tier (Economy, Standard, Premium) to which this rule applies.
    /// </summary>
    /// <value>The <see cref="BudgetLevel"/> tier.</value>
    public BudgetLevel Tier { get; private set; }

    /// <summary>
    /// Gets the expense category (e.g., Food, Accommodation, Transportation).
    /// </summary>
    /// <value>The <see cref="CostCategory"/> classified by this rule.</value>
    public CostCategory Category { get; private set; }

    /// <summary>
    /// Gets the estimated daily expenditure amount for the specified category and tier.
    /// </summary>
    /// <value>The non-negative daily cost amount in the specified currency.</value>
    public decimal AmountPerDay { get; private set; }

    /// <summary>
    /// Gets the three-letter ISO 4217 currency code for the amount.
    /// </summary>
    /// <value>The uppercase currency code (e.g., "EGP", "USD").</value>
    public string Currency { get; private set; } = string.Empty;

    private CostRule()
    {
    }

    private CostRule(
        int cityId,
        BudgetLevel tier,
        CostCategory category,
        decimal amountPerDay,
        string currency)
    {
        CityId = cityId;
        Tier = tier;
        Category = category;
        AmountPerDay = Math.Max(0m, amountPerDay);
        Currency = currency?.Trim().ToUpperInvariant() ?? string.Empty;
    }

    /// <summary>
    /// Factory method to construct a new <see cref="CostRule"/>.
    /// </summary>
    /// <param name="cityId">The identifier of the applicable city.</param>
    /// <param name="tier">The budget tier.</param>
    /// <param name="category">The expense category.</param>
    /// <param name="amountPerDay">Estimated daily amount (must be non-negative).</param>
    /// <param name="currency">The ISO 4217 currency code.</param>
    /// <returns>A new <see cref="CostRule"/> instance.</returns>
    public static CostRule Create(
        int cityId,
        BudgetLevel tier,
        CostCategory category,
        decimal amountPerDay,
        string currency)
    {
        return new CostRule(
            cityId,
            tier,
            category,
            amountPerDay,
            currency);
    }

    /// <summary>
    /// Updates the daily amount and currency for this cost rule.
    /// </summary>
    /// <param name="amountPerDay">The revised daily amount.</param>
    /// <param name="currency">The revised ISO 4217 currency code.</param>
    public void UpdateAmount(decimal amountPerDay, string currency)
    {
        AmountPerDay = Math.Max(0m, amountPerDay);
        Currency = currency?.Trim().ToUpperInvariant() ?? Currency;
    }
}

