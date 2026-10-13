using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Voyara.Application.Abstractions;
using Voyara.SharedKernel.Results;

namespace Voyara.Infrastructure.Currency;

/// <summary>
/// Decorator caching the last known rate per pair and serving it as a fallback
/// when the underlying provider fails.
/// </summary>
public sealed class CachedExchangeRateProvider : IExchangeRateProvider
{
    private readonly IExchangeRateProvider _inner;
    private readonly ILogger<CachedExchangeRateProvider> _logger;
    private readonly ConcurrentDictionary<string, decimal> _lastKnownRates = new();

    /// <summary>Creates the decorator.</summary>
    public CachedExchangeRateProvider(IExchangeRateProvider inner, ILogger<CachedExchangeRateProvider> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<decimal>> GetRateAsync(string baseCurrency, string targetCurrency, CancellationToken cancellationToken = default)
    {
        var result = await _inner.GetRateAsync(baseCurrency, targetCurrency, cancellationToken);

        if (result.IsSuccess)
        {
            _lastKnownRates[CacheKey(baseCurrency, targetCurrency)] = result.Value;
            return result;
        }

        if (_lastKnownRates.TryGetValue(CacheKey(baseCurrency, targetCurrency), out var lastKnown))
        {
            _logger.LogWarning(
                "FX provider failed for {Base}->{Target}; serving last known rate {Rate}",
                baseCurrency,
                targetCurrency,
                lastKnown);
            return Result.Success(lastKnown);
        }

        return result;
    }

    private static string CacheKey(string baseCurrency, string targetCurrency)
        => $"{baseCurrency.ToUpperInvariant()}:{targetCurrency.ToUpperInvariant()}";
}
