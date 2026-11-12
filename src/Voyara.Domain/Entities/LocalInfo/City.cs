using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.LocalInfo;

/// <summary>
/// Represents a geographical city or municipality.
/// </summary>
public sealed class City : AuditableEntity
{
    public new int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string CountryCode { get; private set; } = string.Empty;

    private City()
    {
    }

    private City(string name, string countryCode)
    {
        Name = name;
        CountryCode = countryCode;
    }

    public static City Create(string name, string countryCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(countryCode);

        var normalizedCountryCode = countryCode.Trim().ToUpperInvariant();

        if (normalizedCountryCode.Length != 2 ||
            normalizedCountryCode.Any(c => c is < 'A' or > 'Z'))
        {
            throw new ArgumentException(
                "Country code must contain exactly two English letters.",
                nameof(countryCode));
        }

        return new City(name.Trim(), normalizedCountryCode);
    }

    public void Update(string name, string countryCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(countryCode);

        var normalizedCountryCode = countryCode.Trim().ToUpperInvariant();

        if (normalizedCountryCode.Length != 2 ||
            normalizedCountryCode.Any(c => c is < 'A' or > 'Z'))
        {
            throw new ArgumentException(
                "Country code must contain exactly two English letters.",
                nameof(countryCode));
        }

        Name = name.Trim();
        CountryCode = normalizedCountryCode;
    }
}