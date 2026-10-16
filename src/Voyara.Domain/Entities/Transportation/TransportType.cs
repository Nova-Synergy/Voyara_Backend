using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara.Domain.Entities.Transportation
{
    /// <summary>
    /// Lookup entity that describes a kind of transportation, such as metro, bus or taxi.
    /// </summary>
    public class TransportType
    {
        /// <summary>Primary key of the transport type.</summary>
        public int Id { get; set; }

        /// <summary>Stable, unique machine-readable code (for example <c>METRO</c> or <c>BUS</c>).</summary>
        public string Code { get; set; } = null!;

        /// <summary>Human-readable display name of the transport type.</summary>
        public string Name { get; set; } = null!;

        /// <summary>City-level transport entries that use this transport type (1 to 0..*).</summary>
        public ICollection<CityTransport> CityTransports { get; set; } = new List<CityTransport>();
    }
}
