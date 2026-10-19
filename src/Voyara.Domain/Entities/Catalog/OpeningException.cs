using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Catalog
{
    /// <summary>
    /// Represents a date-specific override of the regular opening hours (holiday, maintenance, ...).
    /// It is created and changed only through its <see cref="Destination"/>.
    /// </summary>
    public sealed class OpeningException : BaseEntity
    {
        internal const int MaxReasonLength = 500;

        /// <summary>
        /// Gets the calendar date the exception applies to.
        /// </summary>
        public DateOnly Date { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the destination is closed on this date.
        /// </summary>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// Gets the reason for the exception.
        /// </summary>
        public string Reason { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the identifier of the owning destination.
        /// </summary>
        public Guid DestinationId { get; private set; }

        /// <summary>
        /// Gets the destination that owns this exception.
        /// </summary>
        public Destination Destination { get; private set; } = null!;

        private OpeningException()
        {
        }

        internal static OpeningException Create(Destination destination, DateOnly date, bool isClosed, string reason)
        {
            ArgumentNullException.ThrowIfNull(destination);

            var exception = new OpeningException { Destination = destination, DestinationId = destination.Id, Date = date };
            exception.Update(isClosed, reason);
            return exception;
        }

        internal void Update(bool isClosed, string reason)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(reason);

            var normalized = reason.Trim();
            if (normalized.Length > MaxReasonLength)
                throw new ArgumentException($"Reason cannot exceed {MaxReasonLength} characters.", nameof(reason));

            IsClosed = isClosed;
            Reason = normalized;
        }
    }
}
