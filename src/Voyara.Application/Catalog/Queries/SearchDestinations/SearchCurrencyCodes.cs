using System;
using System.Collections.Generic;
using System.Linq;

namespace Voyara.Application.Catalog.Queries.SearchDestinations;

/// <summary>
/// Currency codes accepted by the destination-search price filter.
/// Each value is an active ISO 4217 code and is explicitly supported by the current application configuration.
/// Extend this allow-list when currency pricing and exchange-rate support are added to the application.
/// </summary>
public static class SearchCurrencyCodes
{
    private static readonly HashSet<string> SupportedCodes = new(StringComparer.Ordinal)
    {
        "EGP",
        "EUR",
        "GBP",
        "USD"
    };

    /// <summary>Checks whether a value is a currency code supported by destination search.</summary>
    /// <param name="currency">The user-supplied currency value.</param>
    /// <returns><see langword="true"/> only when the normalized code is in the application's supported ISO 4217 set.</returns>
    public static bool IsSupported(string? currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            return false;
        }

        var normalized = currency.Trim().ToUpperInvariant();
        return normalized.Length == 3 && SupportedCodes.Contains(normalized);
    }

    /// <summary>Gets the application-supported search currency codes for validation messages and API documentation.</summary>
    public static IReadOnlyCollection<string> All => SupportedCodes.OrderBy(code => code, StringComparer.Ordinal).ToArray();
}
