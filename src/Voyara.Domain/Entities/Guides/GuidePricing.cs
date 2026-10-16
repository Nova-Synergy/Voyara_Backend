using System;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Guides
{
    /// <summary>
    /// Represents the pricing information for a guide's services.
    /// </summary>
    public sealed class GuidePricing : AuditableEntity
    {
        /// <summary>
        /// Gets the pricing unit for the guide's services (e.g., per hour, per day).
        /// </summary>
        public PricingUnit Unit { get; private set; }

        /// <summary>
        /// Gets the amount charged by the guide for their services.
        /// </summary>
        public decimal Amount { get; private set; }

        /// <summary>
        /// Gets the currency in which the guide's pricing is specified (default is USD).
        /// </summary>
        public string Currency { get; private set; } = "USD";

        /// <summary>
        /// Gets the identifier of the guide profile associated with this pricing information.
        /// </summary>
        public Guid GuideProfileId { get; private set; } 

        /// <summary>
        /// Navigation property to the associated guide profile.
        /// </summary>
        public GuideProfile? GuideProfile { get; private set; }
    }
}
