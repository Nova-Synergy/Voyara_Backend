using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Events;
using Voyara.SharedKernel.Events;

namespace Voyara.Infrastructure.Persistence.Outbox;

/// <summary>
/// Reads pending outbox messages, publishes them as <see cref="DomainEventNotification{TEvent}"/>
/// via MediatR's <see cref="IPublisher"/> and records processing/retry state.
/// </summary>
public sealed class OutboxProcessor : IOutboxProcessor
{
    private static readonly TimeSpan MaxRetryDelay = TimeSpan.FromHours(1);

    private readonly VoyaraDbContext _dbContext;
    private readonly IPublisher _publisher;
    private readonly ILogger<OutboxProcessor> _logger;

    /// <summary>Creates the processor.</summary>
    public OutboxProcessor(VoyaraDbContext dbContext, IPublisher publisher, ILogger<OutboxProcessor> logger)
    {
        _dbContext = dbContext;
        _publisher = publisher;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<int> ProcessPendingAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        var messages = await _dbContext.OutboxMessages
            .Where(m => m.ProcessedAt == null && (m.NextRetryAt == null || m.NextRetryAt <= now))
            .OrderBy(m => m.OccurredOn)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        var published = 0;

        foreach (var message in messages)
        {
            try
            {
                await PublishAsync(message, cancellationToken);
                message.ProcessedAt = DateTimeOffset.UtcNow;
                message.Error = null;
                published++;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                message.Attempts++;
                message.NextRetryAt = DateTimeOffset.UtcNow.Add(
                    TimeSpan.FromSeconds(Math.Min(Math.Pow(2, message.Attempts), MaxRetryDelay.TotalSeconds)));
                message.Error = ex.Message;
                _logger.LogError(ex, "Failed to publish outbox message {OutboxMessageId}", message.Id);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return published;
    }

    private async Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        var eventType = Type.GetType(message.Type);
        if (eventType is null || !typeof(IDomainEvent).IsAssignableFrom(eventType))
        {
            throw new InvalidOperationException($"Unknown domain event type '{message.Type}'.");
        }

        var domainEvent = JsonSerializer.Deserialize(message.Payload, eventType) as IDomainEvent
            ?? throw new InvalidOperationException($"Outbox message {message.Id} has an empty payload.");

        var notificationType = typeof(DomainEventNotification<>).MakeGenericType(eventType);
        var notification = (INotification)Activator.CreateInstance(notificationType, domainEvent)!;

        await _publisher.Publish(notification, cancellationToken);
    }
}
