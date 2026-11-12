using System;
using Voyara.SharedKernel.Entities;
using Voyara.Domain.Enums;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="GuidePricing"/> class.
        /// </summary>
        private GuidePricing()
        {
        }


        /// <summary>
        /// Creates a new pricing entry for a guide profile.
        /// </summary>
        /// <param name="guideProfileId">
        /// The unique identifier of the guide profile.
        /// </param>
        /// <param name="unit">
        /// The pricing unit that determines how the price is calculated.
        /// </param>
        /// <param name="amount">
        /// The price amount, which must be greater than or equal to zero.
        /// </param>
        /// <param name="currency">
        /// The three-letter currency code, consisting of ASCII letters.
        /// </param>
        /// <returns>
        /// A new instance of the <see cref="GuidePricing"/> class with validated and normalized pricing details.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="guideProfileId"/> is <see cref="Guid.Empty"/>,
        /// <paramref name="amount"/> is negative, or <paramref name="currency"/> is not a valid
        /// three-letter ASCII currency code.
        /// </exception>
        internal static GuidePricing Create(Guid guideProfileId, PricingUnit unit, decimal amount, string currency)
        {
            if (guideProfileId == Guid.Empty)
                throw new ArgumentException("A guide profile is required.", nameof(guideProfileId));

            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative.", nameof(amount));

            if (currency is null || currency.Trim().Length != 3 || !currency.Trim().All(char.IsAsciiLetter))
                throw new ArgumentException("Currency must be a 3-letter code (A-Z).", nameof(currency));

            return new GuidePricing
            {
                GuideProfileId = guideProfileId,
                Unit = unit,
                Amount = amount,
                Currency = currency.Trim().ToUpperInvariant()
            };
        }
    }
}