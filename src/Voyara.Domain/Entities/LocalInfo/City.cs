using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.LocalInfo;

/// <summary>
/// Represents a geographical city or municipality referenced across journey waypoints and local cost rules.
/// </summary>
public sealed class City : AuditableEntity
{
    /// <summary>
    /// Gets the database-generated integer identifier of the city.
    /// </summary>
    /// <value>The unique integer key for the city entity.</value>
    public new int Id { get; private set; }

    /// <summary>
    /// Gets the official name of the city.
    /// </summary>
    /// <value>The display name of the municipality.</value>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the two-letter ISO 3166-1 alpha-2 country code where the city resides.
    /// </summary>
    /// <value>The uppercase country code (e.g., "EG", "FR").</value>
    public string CountryCode { get; private set; } = string.Empty;

    private City()
    {
    }

    private City(string name, string countryCode)
    {
        Name = name;
        CountryCode = countryCode;
    }

    /// <summary>
    /// Creates a new <see cref="City"/> entity.
    /// </summary>
    /// <param name="name">The name of the city.</param>
    /// <param name="countryCode">The two-letter ISO country code.</param>
    /// <returns>A new <see cref="City"/> instance.</returns>
    public static City Create(string name, string countryCode) =>
        new(name, countryCode);
}

