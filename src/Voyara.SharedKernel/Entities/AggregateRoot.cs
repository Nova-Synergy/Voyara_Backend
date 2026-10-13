using Voyara.SharedKernel.Events;

namespace Voyara.SharedKernel.Entities;

/// <summary>
/// Base type for aggregate roots; collects domain events raised during a transaction
/// so they can be serialized into the outbox on save.
/// </summary>
public abstract class AggregateRoot : AuditableEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>Domain events raised but not yet persisted to the outbox.</summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>Registers a domain event to be published through the outbox.</summary>
    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>Clears all collected domain events.</summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
