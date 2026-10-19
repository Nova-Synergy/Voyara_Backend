using Voyara.Domain.Entities.Catalog;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Identity
{
    /// <summary>
    /// Represents a record of a destination a tourist visited before.
    /// It is created and removed only through its <see cref="TouristProfile"/>.
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

        private VisitHistory()
        {
        }

        internal static VisitHistory Create(TouristProfile profile, Guid destinationId, DateTime visitedOn)
        {
            ArgumentNullException.ThrowIfNull(profile);

            if (destinationId == Guid.Empty)
                throw new ArgumentException("A destination is required.", nameof(destinationId));

            return new VisitHistory
            {
                TouristProfile = profile,
                TouristProfileId = profile.Id,
                DestinationId = destinationId,
                VisitedOn = visitedOn.Date
            };
        }
    }
}
