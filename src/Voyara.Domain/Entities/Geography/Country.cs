using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Geography
{
    /// <summary>
    /// Represents a country that groups cities and carries the defaults used for its destinations.
    /// </summary>
    public sealed class Country : AuditableEntity
    {
        /// <summary>
        /// Gets the database-generated identifier (hides the Guid Id of the base class).
        /// </summary>
        public new int Id { get; private set; }

        /// <summary>
        /// Gets the ISO 3166-1 alpha-2 country code, for example EG.
        /// </summary>
        public string IsCode { get; private set; } = string.Empty;

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
        /// Gets the cities that belong to the country (0..*).
        /// </summary>
        public ICollection<City> Cities { get; private set; } = new List<City>();

        private Country()
        {
        }

        /// <summary>Creates an active country.</summary>
        /// <param name="isoCode">ISO 3166-1 alpha-2 code, for example EG.</param>
        /// <param name="defaultCurrency">ISO 4217 currency code, for example EGP.</param>
        /// <param name="timeZone">IANA time zone identifier, for example Africa/Cairo.</param>
        /// <returns>The created <see cref="Country"/> instance.</returns>
        /// <exception cref="ArgumentException">An argument is missing or malformed.</exception>
        public static Country Create(string isoCode, string defaultCurrency, string timeZone)
            => new()
            {
                IsCode = NormalizeLetters(isoCode, 2, nameof(isoCode)),
                DefaultCurrency = NormalizeLetters(defaultCurrency, 3, nameof(defaultCurrency)),
                TimeZone = NormalizeTimeZone(timeZone)
            };

        /// <summary>Changes the default currency and the time zone. The ISO code never changes.</summary>
        public void Update(string defaultCurrency, string timeZone)
        {
            DefaultCurrency = NormalizeCode(defaultCurrency, 3, nameof(defaultCurrency));
            TimeZone = NormalizeTimeZone(timeZone);
        }

        /// <summary>Makes the country visible to tourists.</summary>
        /// <exception cref="InvalidOperationException">The country is already active.</exception>
        public void Activate()
        {
            if (IsActive)
                throw new InvalidOperationException("The country is already active.");

            IsActive = true;
        }

        /// <summary>Hides the country from tourists.</summary>
        /// <exception cref="InvalidOperationException">The country is already inactive.</exception>
        public void Deactivate()
        {
            if (!IsActive)
                throw new InvalidOperationException("The country is already inactive.");

            IsActive = false;
        }

        /// <summary>Adds an active city to the country.</summary>
        /// <exception cref="ArgumentOutOfRangeException">A coordinate is outside its valid range.</exception>
        public City AddCity(double latitude, double longitude)
        {
            var city = City.Create(this, latitude, longitude);
            Cities.Add(city);
            return city;
        }

        private static string NormalizeCode(string code, int length, string paramName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code, paramName);

            var normalized = code.Trim().ToUpperInvariant();
            if (normalized.Length != length || !normalized.All(char.IsAsciiLetterUpper))
                throw new ArgumentException($"Value must be a {length}-letter code.", paramName);

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
