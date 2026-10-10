using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.ValueObjects;

namespace Voyara.Domain.Entities.Transportation
{
    /// <summary>
    /// A stop or station on a <see cref="TransportRoute"/>.
    /// </summary>
    public class Station
    {
        /// <summary>Primary key of the station.</summary>
        public Guid Id { get; set; }

        /// <summary>Foreign key to the <see cref="Transportation.TransportRoute"/> this station belongs to.</summary>
        public Guid TransportRouteId { get; set; }

        /// <summary>The route this station belongs to.</summary>
        public TransportRoute TransportRoute { get; set; } = null!;

        /// <summary>Display name of the station.</summary>
        public string Name { get; set; } = null!;

        /// <summary>Geographic location of the station, stored as an owned value object.</summary>
        public GeoPoint Location { get; set; } = null!;
    }
}
