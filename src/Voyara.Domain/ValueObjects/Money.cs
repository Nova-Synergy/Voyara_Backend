using Voyara.SharedKernel.Results;
using Voyara.SharedKernel.ValueObjects;

namespace Voyara.Domain.ValueObjects;

/// <summary>Monetary amount paired with a three-letter ISO 4217 currency code.</summary>
public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }
    /// <summary>
    /// Creates a monetary amount in the specified currency.
    /// </summary>
    /// <param name="amount">The nonnegative amount.</param>
    /// <param name="currency">The currency code, which is trimmed and converted to uppercase.</param>
    /// <returns>A <see cref="Money"/> representing the amount in the specified currency.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The amount is negative.</exception>
    /// <exception cref="ArgumentException">The currency is null, empty, or consists only of white-space characters.</exception>
    public static Money Create(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency is required.");

        return new Money(amount, currency.Trim().ToUpperInvariant());
    }
    /// <summary>Returns a new Money instance with the same currency and the amount added with the given amount.</summary>
    /// <param name="other">The other money amount to add.</param>
    /// <returns>A new Money instance with the combined amount.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the currencies do not match.</exception>
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Currency mismatch.");

        return Create(Amount + other.Amount, Currency);
    }
     /// <summary>
     /// Gets the components used to determine equality.
     /// </summary>
     /// <returns>The amount and currency components, in that order.</returns>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
