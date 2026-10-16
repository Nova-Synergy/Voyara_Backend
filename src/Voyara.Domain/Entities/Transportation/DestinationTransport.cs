using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara.Domain.Entities.Transportation
{
    /// <summary>
    /// Join entity for the many-to-many relationship "Destination is reachable by CityTransport".
    /// </summary>
    /// <remarks>
    /// <see cref="DestinationId"/> is a foreign key to <see cref="Voyara.Domain.Entities.Catalog.Destination"/>.
    /// No navigation property is exposed so the Catalog aggregate stays unaware of Transportation.
    /// The primary key is the composite of <see cref="DestinationId"/> and <see cref="CityTransportId"/>.
    /// </remarks>
    public class DestinationTransport
    {
        /// <summary>Identifier of the destination that can be reached. Part of the primary key.</summary>
        public Guid DestinationId { get; set; }

        /// <summary>Foreign key to the <see cref="Transportation.CityTransport"/> that reaches the destination. Part of the primary key.</summary>
        public Guid CityTransportId { get; set; }

        /// <summary>The city transport entry that reaches the destination.</summary>
        public CityTransport CityTransport { get; set; } = null!;
    }
}
