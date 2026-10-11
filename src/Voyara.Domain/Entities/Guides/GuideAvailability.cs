using System;
using Voyara.SharedKernel.Entities;


namespace Voyara.Domain.Entities.Guides
{
    public sealed class GuideAvailability : AuditableEntity
    {
        /// <summary>
        /// Gets the identifier of the guide profile associated with this availability entry.
        /// </summary>
        public Guid GuideProfileId { get; private set; }

        /// <summary>
        /// Gets the day of the week for which the guide is available.
        /// </summary>
        public DayOfWeek Day { get; private set; }

        /// <summary>
        /// Gets the start time of the guide's availability on the specified day.
        /// </summary>
        public TimeOnly StartTime { get; private set; }

        /// <summary>
        /// Gets the end time of the guide's availability on the specified day.
        /// </summary>
        public TimeOnly EndTime { get; private set; }

        /// <summary>
        /// Navigation property to the associated guide profile.
        /// </summary>
        public GuideProfile? GuideProfile { get; private set; }
    }
}
