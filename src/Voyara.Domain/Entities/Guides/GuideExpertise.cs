using System;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Guides
{
    /// <summary>
    /// Represents a category of expertise associated with a tour guide's profile.
    /// </summary>
    public sealed class GuideExpertise : AuditableEntity
    {
        /// <summary>
        /// Gets the identifier of the guide profile associated with this expertise entry.
        /// </summary>
        public Guid GuideProfileId { get; private set; }

        /// <summary>
        /// Gets the identifier of the category associated with this expertise entry.
        /// </summary>
        public int CategoryId { get; private set; }

        /// <summary>
        /// Gets the guide profile associated with this expertise entry, if available.
        /// </summary>
        public GuideProfile? GuideProfile { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GuideExpertise"/> class.
        /// </summary>
        private GuideExpertise()
        {
        }

        /// <summary>
        /// Creates a new guide expertise associated with a guide profile and a category.
        /// </summary>
        /// <param name="guideProfileId">
        /// The unique identifier of the guide profile.
        /// </param>
        /// <param name="categoryId">
        /// The identifier of the category representing the guide's expertise.
        /// </param>
        /// <returns>
        /// A new instance of the <see cref="GuideExpertise"/> class.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="guideProfileId"/> is <see cref="Guid.Empty"/>
        /// or <paramref name="categoryId"/> is less than or equal to zero.
        /// </exception>
        internal static GuideExpertise Create(Guid guideProfileId, int categoryId)
        {
            if (guideProfileId == Guid.Empty)
                throw new ArgumentException("A guide profile is required.", nameof(guideProfileId));

            if (categoryId <= 0)
                throw new ArgumentException("A category is required.", nameof(categoryId));

            return new GuideExpertise { GuideProfileId = guideProfileId, CategoryId = categoryId };
        }
    }
}