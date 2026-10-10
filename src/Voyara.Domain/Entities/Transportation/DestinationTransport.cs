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
    /// The destination is a <c>«ref»</c> owned by another module
    /// (see <see cref="TravelBridge.Domain.Refs.Destination"/>), so only its Id is stored and no foreign key is created.
    /// The primary key is the composite of <see cref="DestinationId"/> and <see cref="CityTransportId"/>.
    /// </remarks>
    public class DestinationTransport
    {
        /// <summary>Identifier of the destination that can be reached (reference by Id only).</summary>
        public Guid DestinationId { get; set; }

        /// <summary>Foreign key to the <see cref="Transportation.CityTransport"/> that reaches the destination.</summary>
        public Guid CityTransportId { get; set; }

        /// <summary>The city transport entry that reaches the destination.</summary>
        public CityTransport CityTransport { get; set; } = null!;
    }
}
