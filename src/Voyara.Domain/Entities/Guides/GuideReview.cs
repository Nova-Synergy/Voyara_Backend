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
    public class GuideReview : AuditableEntity
    {
        /// <summary>
        /// Gets the identifier of the associated <see cref="GuideRequest"/>.
        /// </summary>
        public Guid GuideRequestId { get; private set; }

        /// <summary>
        /// Gets the identifier of the associated <see cref="GuideProfile"/>.
        /// </summary>
        public Guid GuideProfileId { get; private set; }

        /// <summary>
        /// Gets the numeric rating provided in the review. Expected range depends on application rules.
        /// </summary>
        public int Rating { get; private set; }

        /// <summary>
        /// Gets the textual content of the review.
        /// </summary>
        public string Text { get; private set; } = string.Empty;

        /// <summary>
        /// Navigation property to the associated guide request.
        /// </summary>
        public GuideRequest? GuideRequest { get; private set; }

        /// <summary>
        /// Navigation property to the associated guide profile.
        /// </summary>
        public GuideProfile? GuideProfile { get; private set; }
    }
}
