using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Guides
{
    /// <summary>
    /// Represents a confirmed tour session associated with a guide request.
    /// </summary>
    public sealed class GuideSession : AuditableEntity
    {
        /// <summary>
        /// Gets the identifier of the associated guide request.
        /// </summary>
        public Guid GuideRequestId { get; private set; }

        /// <summary>
        /// Gets the agreed meeting location for the tour.
        /// </summary>
        public string MeetingLocation { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the agreed price for the tour session.
        /// </summary>
        public decimal AgreedPrice { get; private set; }

        /// <summary>
        /// Gets the currency code used for the agreed price.
        /// </summary>
        public string Currency { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the guide request associated with this session.
        /// </summary>
        public GuideRequest GuideRequest { get; private set; } = null!;

        /// <summary>
        /// Initializes a new instance of the GuideSession class. 
        /// </summary>
        /// <remarks>This constructor is private and is intended for internal use only. Instances of
        /// GuideSession cannot be created directly by consumers.</remarks>
        private GuideSession()
        {
        }

        /// <summary>
        /// Creates a new GuideSession instance with the specified request ID, meeting location, agreed price, and
        /// currency.
        /// </summary>
        /// <param name="requestId">The unique identifier of the guide request associated with the session.</param>
        /// <param name="meetingLocation">The location where the meeting will take place. Must not be null, empty, or longer than 500 characters.</param>
        /// <param name="agreedPrice">The price agreed upon for the session. Must be zero or greater.</param>
        /// <param name="currency">The three-letter ISO currency code representing the currency for the agreed price. Must not be null and must
        /// be exactly three characters.</param>
        /// <returns>A GuideSession object initialized with the provided details.</returns>
        /// <exception cref="ArgumentException">Thrown if meetingLocation is null, empty, or longer than 500 characters; if agreedPrice is negative; or if
        /// currency is null or not a three-letter code.</exception>
        internal static GuideSession Create(Guid requestId, string meetingLocation, decimal agreedPrice, string currency)
        {
            if (requestId == Guid.Empty)
                throw new ArgumentException("A guide request is required.", nameof(requestId));

            if (string.IsNullOrWhiteSpace(meetingLocation) || meetingLocation.Trim().Length > 500)
                throw new ArgumentException("Meeting location is required (max 500 characters).", nameof(meetingLocation));

            if (agreedPrice < 0)
                throw new ArgumentException("Price cannot be negative.", nameof(agreedPrice));

            if (currency is null || currency.Trim().Length != 3)
                throw new ArgumentException("Currency must be a 3-letter code.", nameof(currency));

            return new GuideSession
            {
                GuideRequestId = requestId,
                MeetingLocation = meetingLocation.Trim(),
                AgreedPrice = agreedPrice,
                Currency = currency.Trim().ToUpperInvariant()
            };
        }
    }
}