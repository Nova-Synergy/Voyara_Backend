using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Catalog
{
    /// <summary>
    /// Represents the regular opening hours of a destination for one day of the week.
    /// It is created and changed only through its <see cref="Destination"/>.
    /// </summary>
    public sealed class OpeningHour : BaseEntity
    {
        /// <summary>
        /// Gets the day of the week the hours apply to.
        /// </summary>
        public DayOfWeek Day { get; private set; }

        /// <summary>
        /// Gets the time of day the destination opens.
        /// </summary>
        public TimeSpan OpensAt { get; private set; }

        /// <summary>
        /// Gets the time of day the destination closes (may be earlier than opening for overnight venues).
        /// </summary>
        public TimeSpan ClosesAt { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the destination is closed all day.
        /// </summary>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// Gets the identifier of the owning destination.
        /// </summary>
        public Guid DestinationId { get; private set; }

        /// <summary>
        /// Gets the destination that owns these hours.
        /// </summary>
        public Destination Destination { get; private set; } = null!;

        private OpeningHour()
        {
        }

        internal static OpeningHour Create(Destination destination, DayOfWeek day)
        {
            ArgumentNullException.ThrowIfNull(destination);

            if (!Enum.IsDefined(day))
                throw new ArgumentOutOfRangeException(nameof(day), "Day of week is not valid.");

            return new OpeningHour { Destination = destination, DestinationId = destination.Id, Day = day, IsClosed = true };
        }

        internal void Open(TimeSpan opensAt, TimeSpan closesAt)
        {
            if (!IsWithinDay(opensAt))
                throw new ArgumentOutOfRangeException(nameof(opensAt), "Opening time must be within a single day.");

            if (!IsWithinDay(closesAt))
                throw new ArgumentOutOfRangeException(nameof(closesAt), "Closing time must be within a single day.");

            OpensAt = opensAt;
            ClosesAt = closesAt;
            IsClosed = false;
        }

        internal void Close()
        {
            OpensAt = TimeSpan.Zero;
            ClosesAt = TimeSpan.Zero;
            IsClosed = true;
        }

        private static bool IsWithinDay(TimeSpan time) => time >= TimeSpan.Zero && time < TimeSpan.FromDays(1);
    }
}
