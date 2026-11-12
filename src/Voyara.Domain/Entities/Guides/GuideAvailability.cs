using System;
using Voyara.SharedKernel.Entities;


namespace Voyara.Domain.Entities.Guides
{

    /// <summary>
    /// represents the availability schedule of a tour guide, specifying the days and times they are available for bookings.
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="GuideAvailability"/> class.
        /// </summary>
        private GuideAvailability()
        {
        }


        /// <summary>
        /// Creates a new availability schedule for a guide profile.
        /// </summary>
        /// <param name="guideProfileId">
        /// The unique identifier of the guide profile.
        /// </param>
        /// <param name="day">
        /// The day of the week on which the guide is available.
        /// </param>
        /// <param name="start">
        /// The starting time of the guide's availability.
        /// </param>
        /// <param name="end">
        /// The ending time of the guide's availability, which must be later than the starting time.
        /// </param>a
        /// <returns>
        /// A new instance of the <see cref="GuideAvailability"/> class with validated availability details.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="guideProfileId"/> is <see cref="Guid.Empty"/>
        /// or <paramref name="end"/> is earlier than or equal to <paramref name="start"/>.
        /// </exception>
        internal static GuideAvailability Create(Guid guideProfileId, DayOfWeek day, TimeOnly start, TimeOnly end)
        {
            if (guideProfileId == Guid.Empty)
                throw new ArgumentException("A guide profile is required.", nameof(guideProfileId));

            if (end <= start)
                throw new ArgumentException("End time must be after start time.");

            return new GuideAvailability
            {
                GuideProfileId = guideProfileId,
                Day = day,
                StartTime = start,
                EndTime = end
            };
        }
    }
}