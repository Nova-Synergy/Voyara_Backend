using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Guides
{
    /// <summary>
    /// Represents a confirmed session for a guide associated with a <see cref="GuideRequest"/>.
    /// Contains information about meeting location, pricing and the related request.
    /// </summary>
    public class GuideSession : AuditableEntity
    {
        /// <summary>
        /// Gets the identifier of the associated <see cref="GuideRequest"/>.
        /// </summary>
        public Guid GuideRequestId { get; private set; }

        /// <summary>
        /// Gets the agreed meeting location for the session.
        /// </summary>
        public string MeetingLocation { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the agreed price for the session.
        /// </summary>
        public decimal AgreedPrice { get; private set; }

        /// <summary>
        /// Gets the currency code used for the agreed price (e.g. "USD").
        /// </summary>
        public string Currency { get; private set; } = string.Empty;

        /// <summary>
        /// Navigation property to the associated guide request.
        /// </summary>
        public GuideRequest GuideRequest { get; private set; } = null!;
    }
}
