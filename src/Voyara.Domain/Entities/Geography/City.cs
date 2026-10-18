using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Geography
{
    /// <summary>
    /// Represents a city inside a country; destinations are located in cities.
    /// It is created and changed only through its <see cref="Country"/>.
    /// </summary>
    public sealed class City : AuditableEntity
    {

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
        public Guid CountryId { get; private set; }

        /// <summary>
        /// Gets the country that owns this city.
        /// </summary>
        public Country Country { get; private set; } = null!;

        private City()
        {
        }

        internal static City Create(Country country, double latitude, double longitude)
        {
            ArgumentNullException.ThrowIfNull(country);
            EnsureCoordinates(latitude, longitude);

            return new City { Country = country, CountryId = country.Id, Latitude = latitude, Longitude = longitude };
        }

        internal void MoveCenter(double latitude, double longitude)
        {
            EnsureCoordinates(latitude, longitude);
            Latitude = latitude;
            Longitude = longitude;
        }

        internal void Activate()
        {
            if (IsActive)
                throw new InvalidOperationException("The city is already active.");

            IsActive = true;
        }

        internal void Deactivate() => IsActive = false;

        private static void EnsureCoordinates(double latitude, double longitude)
        {
            if (double.IsNaN(latitude) || latitude is < -90 or > 90)
                throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");

            if (double.IsNaN(longitude) || longitude is < -180 or > 180)
                throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");
        }
    }
}
