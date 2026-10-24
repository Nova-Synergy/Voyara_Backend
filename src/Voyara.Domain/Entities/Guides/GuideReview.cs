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

        /// <summary>
        /// Initializes a new instance of the GuideReview class. 
        /// </summary>
        /// <remarks>This constructor is private and is intended for internal use only. Instances of
        /// GuideReview cannot be created directly by consumers.</remarks>
        private GuideReview()
        {
        }

        /// <summary>
        /// Creates a new GuideReview instance with the specified request, guide profile, rating, and review text.  
        /// </summary>
        /// <param name="requestId">The unique identifier of the guide request associated with the review.</param>
        /// <param name="guideProfileId">The unique identifier of the guide profile being reviewed.</param>
        /// <param name="rating">The rating for the guide, from 1 (lowest) to 5 (highest). Must be between 1 and 5.</param>
        /// <param name="text">The review text describing the guide experience. Must not be null, empty, or exceed 2000 characters.</param>
        /// <returns>A GuideReview instance initialized with the provided request ID, guide profile ID, rating, and trimmed
        /// review text.</returns>
        /// <exception cref="ArgumentException">Thrown if rating is less than 1 or greater than 5, or if text is null, empty, or longer than 2000
        /// characters.</exception>
        internal static GuideReview Create(Guid requestId, Guid guideProfileId, int rating, string text)
        {
            if (requestId == Guid.Empty)
                throw new ArgumentException("A guide request is required.", nameof(requestId));

            if (guideProfileId == Guid.Empty)
                throw new ArgumentException("A guide profile is required.", nameof(guideProfileId));

            if (rating is < 1 or > 5)
                throw new ArgumentException("Rating must be between 1 and 5.", nameof(rating));

            if (string.IsNullOrWhiteSpace(text) || text.Trim().Length > 2000)
                throw new ArgumentException("Review text is required (max 2000 characters).", nameof(text));

            return new GuideReview
            {
                GuideRequestId = requestId,
                GuideProfileId = guideProfileId,
                Rating = rating,
                Text = text.Trim()
            };
        }
    }
}