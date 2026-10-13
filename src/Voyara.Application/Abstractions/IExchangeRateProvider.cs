using Voyara.SharedKernel.Results;

namespace Voyara.Application.Abstractions;

/// <summary>Adapter contract for foreign-exchange rate lookups.</summary>
public interface IExchangeRateProvider
{
    /// <summary>Returns the rate to convert one unit of the base currency into the target currency.</summary>
    Task<Result<decimal>> GetRateAsync(string baseCurrency, string targetCurrency, CancellationToken cancellationToken = default);
}
