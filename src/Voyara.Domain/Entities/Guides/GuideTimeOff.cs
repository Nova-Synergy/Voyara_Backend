using System;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Guides
{
    /// <summary>
    /// Represents a date on which a tour guide is unavailable for bookings,
    /// optionally including a reason for the unavailability.
    /// </summary>
    public sealed class GuideTimeOff : AuditableEntity
    {
        /// <summary>
        /// Gets the identifier of the guide profile associated with this time off entry.
        /// </summary>
        public Guid GuideProfileId { get; private set; }

        /// <summary>
        /// Gets the date of the guide's time off.
        /// </summary>
        public DateOnly Date { get; private set; }

        /// <summary>
        /// Gets the reason for the guide's time off, if provided.
        /// </summary>
        public string? Reason { get; private set; }

        /// <summary>
        /// Navigation property to the associated GuideProfile.
        /// </summary>
        public GuideProfile? GuideProfile { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GuideTimeOff"/> class.
        /// </summary>
        private GuideTimeOff()
        {
        }

        /// <summary>
        /// Creates a new time-off entry for a guide profile.
        /// </summary>
        /// <param name="guideProfileId">
        /// The unique identifier of the guide profile.
        /// </param>
        /// <param name="date">
        /// The date on which the guide is unavailable.
        /// </param>
        /// <param name="reason">
        /// An optional reason for the guide's unavailability, which must not exceed 300 characters.
        /// </param>
        /// <returns>
        /// A new instance of the <see cref="GuideTimeOff"/> class with validated and normalized values.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="guideProfileId"/> is <see cref="Guid.Empty"/>
        /// or <paramref name="reason"/> exceeds 300 characters after trimming.
        /// </exception>
        internal static GuideTimeOff Create(Guid guideProfileId, DateOnly date, string? reason)
        {
            if (guideProfileId == Guid.Empty)
                throw new ArgumentException("A guide profile is required.", nameof(guideProfileId));

            var normalizedReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();

            if (normalizedReason is { Length: > 300 })
                throw new ArgumentException("Reason cannot exceed 300 characters.", nameof(reason));

            return new GuideTimeOff
            {
                GuideProfileId = guideProfileId,
                Date = date,
                Reason = normalizedReason
            };
        }
    }
}