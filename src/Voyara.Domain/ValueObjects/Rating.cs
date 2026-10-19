using Voyara.SharedKernel.Results;
using Voyara.SharedKernel.ValueObjects;

namespace Voyara.Domain.ValueObjects;

/// <summary>
/// A star rating from <see cref="Min"/> to <see cref="Max"/>. Modeling it as a value object means an
/// invalid rating can never exist anywhere in the domain, and two ratings with the same value are equal.
/// </summary>
public sealed class Rating : ValueObject
{
    /// <summary>Lowest allowed rating.</summary>
    public const int Min = 1;

    /// <summary>Highest allowed rating.</summary>
    public const int Max = 5;

    /// <summary>The number of stars.</summary>
    public int Value { get; private set; }

    private Rating()
    {
    }

    private Rating(int value) => Value = value;

    /// <summary>Creates a rating, or fails when the value is outside the allowed range.</summary>
    public static Result<Rating> Create(int value)
    {
        if (value < Min || value > Max)
        {
            return Result.Failure<Rating>(Error.Validation(
                "Rating.OutOfRange", $"Rating must be between {Min} and {Max}."));
        }

        return new Rating(value);
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}