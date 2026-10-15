using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Guides
{
    /// <summary>
    /// Represents a tourist's review of a guide following a tour request.
    /// </summary>
    public sealed class GuideReview : AuditableEntity
    {
        /// <summary>
        /// Gets the identifier of the reviewed guide request.
        /// </summary>
        public Guid GuideRequestId { get; private set; }

        /// <summary>
        /// Gets the identifier of the reviewed guide profile.
        /// </summary>
        public Guid GuideProfileId { get; private set; }

        /// <summary>
        /// Gets the rating assigned to the guide.
        /// </summary>
        public int Rating { get; private set; }

        /// <summary>
        /// Gets the review text written by the tourist.
        /// </summary>
        public string Text { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the guide request associated with this review.
        /// </summary>
        public GuideRequest? GuideRequest { get; private set; }

        /// <summary>
        /// Gets the guide profile being reviewed.
        /// </summary>
        public GuideProfile? GuideProfile { get; private set; }
    }
}
