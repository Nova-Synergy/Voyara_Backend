using Voyara.Application.Abstractions;
using Voyara.SharedKernel.Results;

namespace Voyara.Infrastructure.Currency;

/// <summary>Stub FX provider with a few canned rates; returns a failure for unknown pairs.</summary>
public sealed class StubExchangeRateProvider : IExchangeRateProvider
{
    private static readonly IReadOnlyDictionary<string, decimal> Rates = new Dictionary<string, decimal>
    {
        ["USD:EUR"] = 0.92m,
        ["EUR:USD"] = 1.09m,
        ["USD:GBP"] = 0.79m,
        ["GBP:USD"] = 1.27m
    };

    /// <inheritdoc />
    public Task<Result<decimal>> GetRateAsync(string baseCurrency, string targetCurrency, CancellationToken cancellationToken = default)
    {
        if (string.Equals(baseCurrency, targetCurrency, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(Result.Success(1.0m));
        }

        return Rates.TryGetValue($"{baseCurrency.ToUpperInvariant()}:{targetCurrency.ToUpperInvariant()}", out var rate)
            ? Task.FromResult(Result.Success(rate))
            : Task.FromResult(Result.Failure<decimal>(Error.Failure(
                "Fx.RateUnavailable", $"No rate available for {baseCurrency}->{targetCurrency}.")));
    }
}
