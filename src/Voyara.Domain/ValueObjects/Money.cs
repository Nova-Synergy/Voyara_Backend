using Voyara.SharedKernel.Results;
using Voyara.SharedKernel.ValueObjects;

namespace Voyara.Domain.ValueObjects;

/// <summary>Monetary amount paired with a three-letter ISO 4217 currency code.</summary>
public sealed class Money : ValueObject
{
    /// <summary>Amount in the currency's major unit.</summary>
    public decimal Amount { get; private set; }

    /// <summary>ISO 4217 currency code (e.g. "USD").</summary>
    public string Currency { get; private set; } = string.Empty;

    private Money()
    {
    }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    /// <summary>Creates a validated monetary amount.</summary>
    public static Result<Money> Create(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3 || !currency.All(c => char.IsLetter(c) && char.IsUpper(c)))
        {
            return Result.Failure<Money>(Error.Validation(
                "Money.InvalidCurrency", "Currency must be a three-letter uppercase ISO 4217 code."));
        }

        if (amount < 0)
        {
            return Result.Failure<Money>(Error.Validation(
                "Money.NegativeAmount", "Amount cannot be negative."));
        }

        return new Money(amount, currency);
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
