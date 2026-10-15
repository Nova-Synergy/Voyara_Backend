using Voyara.Domain.Enums;
using Voyara.SharedKernel.Results;
using Voyara.SharedKernel.ValueObjects;

namespace Voyara.Domain.ValueObjects;

/// <summary>
/// Value object representing an estimated expense range for a specific spending category.
/// </summary>
/// <remarks>
/// Pairs a <see cref="CostCategory"/> with minimum and maximum projected amounts.
/// Enforces the invariant that the minimum cost must be non-negative and the maximum cost must be greater than or equal to the minimum.
/// </remarks>
public sealed class CostLine : ValueObject
{
    /// <summary>
    /// Gets the expense category for this cost projection.
    /// </summary>
    /// <value>The <see cref="CostCategory"/> being estimated.</value>
    public CostCategory Category { get; private set; }

    /// <summary>
    /// Gets the minimum expected cost for the category.
    /// </summary>
    /// <value>The non-negative lower bound in the associated currency.</value>
    public decimal Min { get; private set; }

    /// <summary>
    /// Gets the maximum expected cost for the category.
    /// </summary>
    /// <value>The non-negative upper bound, greater than or equal to <see cref="Min"/>.</value>
    public decimal Max { get; private set; }

    private CostLine()
    {
    }

    private CostLine(CostCategory category, decimal min, decimal max)
    {
        Category = category;
        Min = min;
        Max = max;
    }

    /// <summary>
    /// Creates a validated <see cref="CostLine"/> value object.
    /// </summary>
    /// <param name="category">The expense category.</param>
    /// <param name="min">The minimum projected cost, which must be non-negative.</param>
    /// <param name="max">The maximum projected cost, which must not be less than <paramref name="min"/>.</param>
    /// <returns>A <see cref="Result{TValue}"/> containing the valid <see cref="CostLine"/> or a validation error.</returns>
    public static Result<CostLine> Create(CostCategory category, decimal min, decimal max)
    {
        if (min < 0)
        {
            return Result.Failure<CostLine>(Error.Validation(
                "CostLine.NegativeMin", "Minimum cost cannot be negative."));
        }

        if (max < min)
        {
            return Result.Failure<CostLine>(Error.Validation(
                "CostLine.InvalidRange", "Maximum cost must be greater than or equal to the minimum cost."));
        }

        return new CostLine(category, min, max);
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Category;
        yield return Min;
        yield return Max;
    }
}

