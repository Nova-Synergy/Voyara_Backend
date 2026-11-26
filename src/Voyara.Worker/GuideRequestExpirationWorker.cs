using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Application.Guides;
using Voyara.Application.Guides.Commands.ExpireOverdueGuideRequests;

namespace Voyara.Worker
{
    /// <summary>
    /// Periodically expires pending guide requests whose <c>ExpiresAt</c> has passed. The work itself lives in
    /// <see cref="ExpireOverdueGuideRequestsCommand"/>, so it can also be invoked without this scheduler.
    /// </summary>
    public sealed class GuideRequestExpirationWorker : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<GuideRequestExpirationWorker> _logger;

        public GuideRequestExpirationWorker(IServiceScopeFactory scopeFactory, ILogger<GuideRequestExpirationWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);

            try
            {
                do
                {
                    await RunOnceAsync(stoppingToken);
                }
                while (await timer.WaitForNextTickAsync(stoppingToken));
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Normal shutdown.
            }
        }

        private async Task RunOnceAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Drain in batches; each batch is its own scope (own DbContext) and its own transaction.
                while (!cancellationToken.IsCancellationRequested)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var sender = scope.ServiceProvider.GetRequiredService<ISender>();

                    var result = await sender.Send(new ExpireOverdueGuideRequestsCommand(), cancellationToken);
                    if (result.IsFailure)
                    {
                        _logger.LogError("Guide request expiration failed: {Error}", result.Error);
                        return;
                    }

                    if (result.Value > 0)
                    {
                        _logger.LogInformation("Expired {Count} overdue guide requests.", result.Value);
                    }

                    if (result.Value < GuideRequestPolicy.ExpirationBatchSize)
                    {
                        return;
                    }
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                // A failed batch rolled back as a whole; the next tick retries with fresh data.
                _logger.LogError(exception, "Guide request expiration run failed; it will be retried on the next cycle.");
            }
        }
    }

}
