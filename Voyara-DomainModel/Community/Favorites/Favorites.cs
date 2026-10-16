using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara_DomainModel.Community.Favorites
{
    /// <summary>
    /// A tourist's saved destination. It has no id of its own: a favorite is identified by
    /// the pair (<see cref="UserId"/>, <see cref="DestinationId"/>), which also prevents
    /// saving the same destination twice. Both ends belong to other modules, so only
    /// their ids are stored.
    /// </summary>
    public class Favorites
    {
        /// <summary>Id of the tourist who saved the destination.</summary>
        public Guid UserId { get; private set; }

        /// <summary>
        ///  Id of the destination the tourist saved.
        /// </summary>
        public Guid DestinationId { get; private set; }

        /// <summary>When the destination was saved (UTC).</summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>Creates a favorite.</summary>
        /// <param name="userId">The tourist's id.</param>
        /// <param name="destinationId">The destination's id.</param>
        public Favorites(Guid userId, Guid destinationId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId required.", nameof(userId));
            UserId = userId;

            if (destinationId == Guid.Empty)
                throw new ArgumentException("DestinationId required.", nameof(destinationId));
            DestinationId = destinationId;

            CreatedAt = DateTime.UtcNow;
        }

    }
}
