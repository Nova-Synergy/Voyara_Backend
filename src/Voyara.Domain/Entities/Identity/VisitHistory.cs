using Voyara.Domain.Entities.Catalog;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Identity
{
    /// <summary>
    /// Represents a record of a destination a tourist visited before.
    /// </summary>
    public sealed class VisitHistory : BaseEntity
    {
        /// <summary>
        /// Gets the day of the visit.
        /// </summary>
        public DateTime VisitedOn { get; private set; }

        /// <summary>
        /// Gets the identifier of the tourist profile that owns the record.
        /// </summary>
        public Guid TouristProfileId { get; private set; }

        /// <summary>
        /// Gets the tourist profile that owns the record.
        /// </summary>
        public TouristProfile TouristProfile { get; private set; } = null!;

        /// <summary>
        /// Gets the identifier of the visited destination.
        /// </summary>
        public Guid DestinationId { get; private set; }

        /// <summary>
        /// Gets the visited destination.
        /// </summary>
        public Destination Destination { get; private set; } = null!;
    }
}
