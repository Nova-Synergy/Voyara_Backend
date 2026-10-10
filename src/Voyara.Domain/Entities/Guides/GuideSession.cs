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
    }
}
