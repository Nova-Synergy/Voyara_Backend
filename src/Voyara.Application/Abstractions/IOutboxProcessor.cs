namespace Voyara.Application.Abstractions;

/// <summary>Processes pending transactional-outbox messages.</summary>
public interface IOutboxProcessor
{
    /// <summary>Publishes up to <paramref name="batchSize"/> pending messages; returns the processed count.</summary>
    Task<int> ProcessPendingAsync(int batchSize, CancellationToken cancellationToken = default);
}
