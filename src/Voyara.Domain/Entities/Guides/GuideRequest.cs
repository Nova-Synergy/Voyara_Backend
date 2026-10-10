using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Enums;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Guides
{
    /// <summary>
    /// Represents a tourist's request to book a tour with a guide.
    /// </summary>
    public class GuideRequest : AuditableEntity
    {
        /// <summary>
        /// Gets the identifier of the tourist who created the request.
        /// </summary>
        public Guid TouristUserId { get; private set; }

        /// <summary>
        /// Gets the identifier of the guide profile that the tourist requested.
        /// </summary>
        public Guid GuideProfileId { get; private set; }

        /// <summary>
        /// Gets the identifier of the destination for the requested tour.
        /// </summary>
        public Guid DestinationId { get; private set; }

        /// <summary>
        /// Gets the date of the requested tour.
        /// </summary>
        public DateOnly Date { get; private set; }

        /// <summary>
        /// Gets the requested start time for the tour.
        /// </summary>
        public TimeOnly StartTime { get; private set; }

        /// <summary>
        /// Gets the requested duration of the tour in hours. Can be fractional.
        /// </summary>
        public decimal DurationHours { get; private set; }

        /// <summary>
        /// Gets the number of people in the tourist's party.
        /// </summary>
        public int PartySize { get; private set; }

        /// <summary>
        /// Gets the preferred language for the tour (e.g. "en", "ar").
        /// </summary>
        public string PreferredLanguage { get; private set; } = string.Empty;

        /// <summary>
        /// Gets any special requirements or notes provided by the tourist.
        /// </summary>
        public string? SpecialRequirements { get; private set; }

        /// <summary>
        /// Gets the current status of the request.
        /// </summary>
        public RequestStatus Status { get; private set; } = RequestStatus.Pending;

        /// <summary>
        /// Gets the reason provided when the request was rejected, if any.
        /// </summary>
        public string? RejectionReason { get; private set; }

        /// <summary>
        /// Gets the date and time when the request will expire.
        /// </summary>
        public DateTimeOffset ExpiresAt { get; private set; }

        /// <summary>
        /// Navigation property to the associated guide profile.
        /// </summary>
        public GuideProfile? GuideProfile { get; private set; }

        /// <summary>
        /// Navigation property to the associated GuideSession.
        /// </summary>
        public GuideSession? Session { get; private set; }

        /// <summary>
        /// Navigation property to the associated chat thread.
        /// </summary>
        public ChatThread? ChatThread { get; private set; }

        /// <summary>
        /// Navigation property to the associated guide review.
        /// </summary>
        public GuideReview? Review { get; private set; }

        /// <summary>
        /// Navigation property to the associated destination.
        /// </summary>
        public Destination? Destination { get; private set; }
    }
}
