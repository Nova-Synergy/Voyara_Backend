using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Voyara.Infrastructure.Persistence.Outbox;
using Voyara.SharedKernel.Entities;

namespace Voyara.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Serializes domain events collected on aggregate roots into <see cref="OutboxMessage"/> rows
/// within the same transaction. It never publishes events directly.
/// </summary>
public sealed class OutboxInterceptor : SaveChangesInterceptor
{
    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        QueueDomainEvents(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        QueueDomainEvents(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    private static void QueueDomainEvents(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var aggregatesWithEvents = context.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(entry => entry.Entity.DomainEvents.Count > 0)
            .Select(entry => entry.Entity)
            .ToList();

        var messages = aggregatesWithEvents
            .SelectMany(aggregate => aggregate.DomainEvents.Select(domainEvent => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOn = domainEvent.OccurredOn,
                Type = domainEvent.GetType().AssemblyQualifiedName
                    ?? domainEvent.GetType().FullName!,
                Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
            }))
            .ToList();

        if (messages.Count > 0)
        {
            context.Set<OutboxMessage>().AddRange(messages);
            aggregatesWithEvents.ForEach(aggregate => aggregate.ClearDomainEvents());
        }
    }
}
