using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Entities.Catalog;
using CatalogDestination = Voyara.Domain.Entities.Catalog.Destination;
using Voyara.Domain.Refrence_Class;
namespace Voyara.Domain.Entities.Transportation
{
    /// <summary>
    /// Describes how a specific <see cref="TransportType"/> works inside a specific city:
    /// cost, operating hours, safety information and the routes it offers.
    /// </summary>
    /// <remarks>
    /// Acts as the aggregate root of the Transportation package. It owns its <see cref="Routes"/> (composition)
    /// and is linked to destinations through <see cref="Destinations"/>.
    /// </remarks>
    public class CityTransport
    {
        /// <summary>Primary key of the city transport entry.</summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identifier of the city this entry belongs to. The city is a <c>«ref»</c> owned by another module
        /// (see <see cref="TravelBridge.Domain.Refs.City"/>), so only its Id is stored and no foreign key is created.
        /// </summary>
        public int CityId { get; set; }

        /// <summary>Foreign key to the <see cref="Transportation.TransportType"/>.</summary>
        public int TransportTypeId { get; set; }

        /// <summary>The kind of transportation described by this entry.</summary>
        public TransportType TransportType { get; set; } = null!;

        /// <summary>General description of the transport option in the city.</summary>
        public string Description { get; set; } = null!;

        /// <summary>Approximate cost of a typical ride, expressed in <see cref="Currency"/>.</summary>
        public decimal ApproxCost { get; set; }

        /// <summary>ISO 4217 currency code of <see cref="ApproxCost"/> (for example <c>EGP</c>).</summary>
        public string Currency { get; set; } = "EGP";

        /// <summary>Free-text operating hours (for example <c>05:00 - 24:00</c>).</summary>
        public string OperatingHours { get; set; } = null!;

        /// <summary>Safety tips and warnings related to using this transport option.</summary>
        public string SafetyInfo { get; set; } = null!;

        /// <summary>Date and time (UTC recommended) of the last update to this entry.</summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>Routes offered by this transport option. Composition: routes cannot exist without their parent.</summary>
        public ICollection<TransportRoute> Routes { get; set; } = new List<TransportRoute>();

        /// <summary>
        /// Links to the destinations that are reachable by this transport option (many-to-many through
        /// <see cref="DestinationTransport"/>).
        /// </summary>
        public ICollection<DestinationTransport> Destinations { get; set; } = new List<DestinationTransport>();
    }
}
