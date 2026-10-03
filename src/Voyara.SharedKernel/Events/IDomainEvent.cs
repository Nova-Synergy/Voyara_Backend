namespace Voyara.SharedKernel.Events;

/// <summary>
/// Marker contract for domain events. Deliberately free of any mediator dependency;
/// the Application layer wraps events in its own notification types.
/// </summary>
public interface IDomainEvent
{
    /// <summary>Unique identifier of the event occurrence.</summary>
    Guid EventId { get; }

    /// <summary>UTC moment the event was raised.</summary>
    DateTimeOffset OccurredOn { get; }
}
