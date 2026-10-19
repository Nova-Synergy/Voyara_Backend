using System;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Guides
{
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
    }
}
