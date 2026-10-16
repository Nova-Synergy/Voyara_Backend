using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Geography
{
    /// <summary>
    /// Represents a city inside a country; destinations are located in cities.
    /// </summary>
    public sealed class City : AuditableEntity
    {
        /// <summary>
        /// Gets the database-generated identifier (hides the Guid Id of the base class).
        /// </summary>
        public new int Id { get; private set; }

        /// <summary>
        /// Gets the latitude of the city center in degrees.
        /// </summary>
        public double Latitude { get; private set; }

        /// <summary>
        /// Gets the longitude of the city center in degrees.
        /// </summary>
        public double Longitude { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the city is visible to tourists.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Gets the identifier of the owning country.
        /// </summary>
        public int CountryId { get; private set; }

        /// <summary>
        /// Gets the country that owns this city.
        /// </summary>
        public Country Country { get; private set; } = null!;
    }
}
