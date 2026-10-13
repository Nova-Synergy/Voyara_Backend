using Voyara.SharedKernel.Events;

namespace Voyara.Domain.Events.Catalog;

/// <summary>Raised when a destination transitions from Draft to Published.</summary>
public sealed record DestinationPublishedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid DestinationId,
    string Name) : IDomainEvent;
