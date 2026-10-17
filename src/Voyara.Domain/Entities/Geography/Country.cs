using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// Gets the cities that belong to the country (0..*).
        /// </summary>
        public ICollection<City> Cities { get; private set; } = new List<City>();
    }
}
