using System;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Guides
{
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
        /// Navigation property to the associated guide profile.
        /// </summary>
        public GuideProfile? GuideProfile { get; private set; }

        /// <summary>
        /// Navigation property to the associated category.
        /// </summary>
        public Category? Category { get; private set; }
    }
}
