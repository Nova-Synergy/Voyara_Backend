using System;
using Voyara.Domain.ValueObjects;
using Voyara.SharedKernel.Entities;
using Voyara.Domain.Enums;

namespace Voyara.Domain.Entities.Catalog
{
    /// <summary>
    /// Represents the entrance price of a destination for one kind of visitor.
    /// It is created and changed only through its <see cref="Destination"/>.
    /// </summary>
    public sealed class TicketPrice : BaseEntity
    {
        internal const int MaxNotesLength = 500;

        /// <summary>
        /// Gets the kind of visitor the price applies to.
        /// </summary>
        public VisitorType VisitorType { get; private set; }

        /// <summary>
        /// Gets the ticket price.
        /// </summary>
        public Money Price { get; private set; } = null!;

        /// <summary>
        /// Gets extra information about the ticket.
        /// </summary>
        public string Notes { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the identifier of the owning destination.
        /// </summary>
        public Guid DestinationId { get; private set; }

        /// <summary>
        /// Gets the destination that owns this price.
        /// </summary>
        public Destination Destination { get; private set; } = null!;

        private TicketPrice()
        {
        }

        internal static TicketPrice Create(Destination destination, VisitorType visitorType, Money price, string? notes)
        {
            ArgumentNullException.ThrowIfNull(destination);

            if (!Enum.IsDefined(visitorType))
                throw new ArgumentOutOfRangeException(nameof(visitorType), "Visitor type is not valid.");

            var ticket = new TicketPrice { Destination = destination, DestinationId = destination.Id, VisitorType = visitorType };
            ticket.Update(price, notes);
            return ticket;
        }

        internal void Update(Money price, string? notes)
        {
            ArgumentNullException.ThrowIfNull(price);

            var normalized = notes?.Trim() ?? string.Empty;
            if (normalized.Length > MaxNotesLength)
                throw new ArgumentException($"Notes cannot exceed {MaxNotesLength} characters.", nameof(notes));

            Price = price;
            Notes = normalized;
        }
    }
}
