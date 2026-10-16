using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara.Domain.Entities.Transportation
{
    /// <summary>
    /// A named route that belongs to a <see cref="CityTransport"/> and passes through a series of <see cref="Station"/>s.
    /// </summary>
    public class TransportRoute
    {
        /// <summary>Primary key of the route.</summary>
        public Guid Id { get; set; }

        /// <summary>Foreign key to the owning <see cref="Transportation.CityTransport"/>.</summary>
        public Guid CityTransportId { get; set; }

        /// <summary>The city transport entry that owns this route.</summary>
        public CityTransport CityTransport { get; set; } = null!;

        /// <summary>Display name of the route (for example <c>Line 1: Helwan - New El-Marg</c>).</summary>
        public string Name { get; set; } = null!;

        /// <summary>Description of the route and any useful details for travelers.</summary>
        public string Description { get; set; } = null!;

        /// <summary>Fare for the whole route, or <see langword="null"/> when the fare is unknown or variable.</summary>
        public decimal? Fare { get; set; }

        /// <summary>
        /// Stations along the route. A route must contain at least two stations (2..*); EF Core does not enforce
        /// this rule, so it must be validated in the application layer.
        /// </summary>
        public ICollection<Station> Stations { get; set; } = new List<Station>();
    }
}
