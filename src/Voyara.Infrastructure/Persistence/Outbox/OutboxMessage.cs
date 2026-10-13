namespace Voyara.Infrastructure.Persistence.Outbox;

/// <summary>A serialized domain event persisted in the same transaction as its aggregate.</summary>
public sealed class OutboxMessage
{
    /// <summary>Identifier of the outbox message.</summary>
    public Guid Id { get; set; }

    /// <summary>UTC moment the domain event occurred.</summary>
    public DateTimeOffset OccurredOn { get; set; }

    /// <summary>Assembly-qualified type name of the domain event.</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>JSON-serialized domain event payload.</summary>
    public string Payload { get; set; } = string.Empty;

    /// <summary>Publication attempts so far.</summary>
    public int Attempts { get; set; }

    /// <summary>Earliest moment a failed message is retried; null when no retry is scheduled.</summary>
    public DateTimeOffset? NextRetryAt { get; set; }

    /// <summary>UTC moment the event was successfully published; null while pending.</summary>
    public DateTimeOffset? ProcessedAt { get; set; }

    /// <summary>Last publication error, if any.</summary>
    public string? Error { get; set; }
}
