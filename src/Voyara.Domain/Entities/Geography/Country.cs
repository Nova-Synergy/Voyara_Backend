using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Geography
{
    /// <summary>
    /// Represents a country that groups cities and carries the defaults used for its destinations.
    /// It composes its cities: they are created and changed only through the country.
    /// </summary>
    public sealed class Country : AuditableEntity
    {
        private readonly List<City> _cities = [];

        /// <summary>
        /// Gets the ISO 3166-1 alpha-2 country code, for example EG.
        /// </summary>
        public string IsoCode { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the ISO 4217 currency code used by default, for example EGP.
        /// </summary>
        public string DefaultCurrency { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the IANA time zone identifier, for example Africa/Cairo.
        /// </summary>
        public string TimeZone { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the country is visible to tourists.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Read-only view of the cities (1..*). Cities are added only through <see cref="AddCity"/>.
        /// </summary>
        public IReadOnlyCollection<City> Cities => _cities.AsReadOnly();

        private Country()
        {
        }

        /// <summary>
        /// Creates a country. It stays hidden from tourists until <see cref="Activate"/> is called.
        /// </summary>
        /// <param name="isoCode">Two-letter ISO 3166-1 code.</param>
        /// <param name="defaultCurrency">Three-letter ISO 4217 code.</param>
        /// <param name="timeZone">IANA time zone identifier.</param>
        /// <returns>The created <see cref="Country"/> instance.</returns>
        /// <exception cref="ArgumentException">A value is missing or malformed.</exception>
        public static Country Create(string isoCode, string defaultCurrency, string timeZone)
        {
            return new Country
            {
                IsoCode = NormalizeLetters(isoCode, 2, nameof(isoCode)),
                DefaultCurrency = NormalizeLetters(defaultCurrency, 3, nameof(defaultCurrency)),
                TimeZone = NormalizeTimeZone(timeZone)
            };
        }

        /// <summary>Changes the default currency.</summary>
        public void ChangeDefaultCurrency(string defaultCurrency)
            => DefaultCurrency = NormalizeLetters(defaultCurrency, 3, nameof(defaultCurrency));

        /// <summary>Changes the IANA time zone.</summary>
        public void ChangeTimeZone(string timeZone) => TimeZone = NormalizeTimeZone(timeZone);

        /// <summary>Makes the country visible to tourists.</summary>
        /// <exception cref="InvalidOperationException">The country is already active.</exception>
        public void Activate()
        {
            if (IsActive)
                throw new InvalidOperationException("The country is already active.");

            IsActive = true;
        }

        /// <summary>Hides the country from tourists together with all of its cities.</summary>
        /// <exception cref="InvalidOperationException">The country is already inactive.</exception>
        public void Deactivate()
        {
            if (!IsActive)
                throw new InvalidOperationException("The country is already inactive.");

            IsActive = false;
            _cities.ForEach(city => city.Deactivate());
        }

        /// <summary>Adds a city to the country; the city stays hidden until <see cref="ActivateCity"/> is called.</summary>
        /// <exception cref="ArgumentOutOfRangeException">The coordinates are out of range.</exception>
        public City AddCity(double latitude, double longitude)
        {
            var city = City.Create(this, latitude, longitude);
            _cities.Add(city);
            return city;
        }

        /// <summary>Makes a city visible to tourists. Only possible while the country itself is active.</summary>
        /// <exception cref="InvalidOperationException">The country is inactive or the city is not part of it.</exception>
        public void ActivateCity(Guid cityId)
        {
            if (!IsActive)
                throw new InvalidOperationException("A city cannot be activated inside an inactive country.");

            FindCity(cityId).Activate();
        }

        /// <summary>Hides a city from tourists.</summary>
        /// <exception cref="InvalidOperationException">The city is not part of this country.</exception>
        public void DeactivateCity(Guid cityId) => FindCity(cityId).Deactivate();

        /// <summary>Moves the center of a city.</summary>
        /// <exception cref="InvalidOperationException">The city is not part of this country.</exception>
        public void MoveCityCenter(Guid cityId, double latitude, double longitude)
            => FindCity(cityId).MoveCenter(latitude, longitude);

        private City FindCity(Guid cityId)
            => _cities.Find(city => city.Id == cityId)
               ?? throw new InvalidOperationException("The city is not part of this country.");

        private static string NormalizeLetters(string value, int length, string parameterName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);

            var normalized = value.Trim().ToUpperInvariant();
            if (normalized.Length != length || !normalized.All(char.IsAsciiLetterUpper))
                throw new ArgumentException($"Value must be exactly {length} letters.", parameterName);

            return normalized;
        }

        private static string NormalizeTimeZone(string timeZone)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(timeZone);

            var normalized = timeZone.Trim();
            if (normalized.Length > 64)
                throw new ArgumentException("Time zone cannot exceed 64 characters.", nameof(timeZone));

            return normalized;
        }
    }
}
