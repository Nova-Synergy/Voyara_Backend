using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara_DomainModel.Common
{
    /// <summary>
    /// Base class for an aggregate root: the "boss" entity of a small cluster of
    /// related objects. Outside code may only change the cluster through the root,
    /// which guarantees the business rules are always enforced.
    /// An aggregate root also collects the <see cref="IDomainEvent"/>s it raises,
    /// so the infrastructure layer can publish them after the data is saved.
    /// </summary>
    /// <typeparam name="TId">The type of the identifier (usually <see cref="Guid"/>).</typeparam>
    public abstract class AggregateRoot<TId> : Entity<TId>
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        /// <summary>Events raised by this aggregate that have not been published yet.</summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

        /// <summary>Records that something happened. Called by the aggregate's own methods.</summary>
        /// <param name="domainEvent">The event to record.</param>
        protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

        /// <summary>Clears the recorded events. Called after they have been published.</summary>
        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
