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
    public sealed class GuideRequest : AggregateRoot
    {
        /// <summary>
        /// Gets the requested tour date.
        /// </summary>
        public DateOnly Date { get; private set; }

        /// <summary>
        /// Gets the requested tour start time.
        /// </summary>
        public TimeOnly StartTime { get; private set; }

        /// <summary>
        /// Gets the requested tour duration in hours.
        /// </summary>
        public int DurationHours { get; private set; }

        /// <summary>
        /// Gets the number of tourists included in the request.
        /// </summary>
        public int PartySize { get; private set; }

        /// <summary>
        /// Gets the tourist's preferred language for the tour.
        /// </summary>
        public string PreferredLanguage { get; private set; } = string.Empty;

        /// <summary>
        /// Gets any additional requirements provided by the tourist.
        /// </summary>
        public string? SpecialRequirements { get; private set; }

        /// <summary>
        /// Gets the current status of the guide request.
        /// </summary>
        public RequestStatus Status { get; private set; } = RequestStatus.Pending;

        /// <summary>
        /// Gets the reason provided when the request is rejected, if applicable.
        /// </summary>
        public string? RejectionReason { get; private set; }

        /// <summary>
        /// Gets the expiration date and time of the request.
        /// </summary>
        public DateTimeOffset ExpiresAt { get; private set; }

        /// <summary>
        /// Gets the identifier of the tourist who created the request.
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Gets the identifier of the requested guide profile.
        /// </summary>
        public Guid GuideProfileId { get; private set; }

        /// <summary>
        /// Gets the guide profile associated with this request.
        /// </summary>
        public GuideProfile? GuideProfile { get; private set; }

        /// <summary>
        /// Gets the optional session associated with this request.
        /// </summary>
        public GuideSession? GuideSession { get; private set; }

        /// <summary>
        /// Gets the chat threads associated with this request.
        /// </summary>
        public ICollection<GuideChatThread> GuideChatThreads { get; private set; } = new List<GuideChatThread>();
        
        /// <summary>
        /// Gets the optional review associated with this request.
        /// </summary>
        public GuideReview? GuideReview { get; private set; }

        /// <summary>
        /// Gets the tourist user associated with this request.
        /// </summary>
        public User? User { get; private set; }

        /// <summary>
        /// Gets the destinations associated with this request.
        /// </summary>
        public ICollection<Destination> Destinations { get; private set; } = new List<Destination>();
    }
}
