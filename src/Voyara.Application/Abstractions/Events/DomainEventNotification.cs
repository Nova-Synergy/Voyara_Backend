using MediatR;
using Voyara.SharedKernel.Events;

namespace Voyara.Application.Abstractions.Events;

/// <summary>
/// MediatR notification wrapper around a domain event, published by the outbox processor.
/// </summary>
/// <typeparam name="TEvent">The wrapped domain event type.</typeparam>
public sealed record DomainEventNotification<TEvent>(TEvent DomainEvent) : INotification
    where TEvent : IDomainEvent;
