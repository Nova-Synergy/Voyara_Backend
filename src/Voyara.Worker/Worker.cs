
using Voyara.Application.Abstractions;

namespace Voyara.Worker;

/// <summary>
/// Background service that periodically processes pending transactional outbox messages.
/// </summary>
public sealed class Worker : BackgroundService
{
    private const int BatchSize = 20;

    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<Worker> _logger;

    /// <summary>
    /// Creates the outbox worker.
    /// </summary>
    public Worker(IServiceScopeFactory scopeFactory, ILogger<Worker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Create a fresh scope for each polling cycle.
                // The DbContext and outbox processor are scoped services.
                await using var scope = _scopeFactory.CreateAsyncScope();

                var outboxProcessor = scope.ServiceProvider
                    .GetRequiredService<IOutboxProcessor>();

                var processedCount = await outboxProcessor.ProcessPendingAsync(
                    BatchSize,
                    stoppingToken);

                if (processedCount > 0)
                {
                    _logger.LogInformation(
                        "Processed {ProcessedCount} outbox messages.",
                        processedCount);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while processing outbox messages.");
            }

            try
            {
                await Task.Delay(PollingInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation("Outbox worker stopped.");
    }
}
