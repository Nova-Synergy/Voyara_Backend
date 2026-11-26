using MediatR;
using Microsoft.Extensions.Logging;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Commands.ExpireOverdueGuideRequests;

public sealed class ExpireOverdueGuideRequestsCommandHandler
    : IRequestHandler<ExpireOverdueGuideRequestsCommand, Result<int>>
{
    private readonly IGuideRequestRepository _requests;
    private readonly IDateTime _dateTime;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ExpireOverdueGuideRequestsCommandHandler> _logger;

    public ExpireOverdueGuideRequestsCommandHandler(
        IGuideRequestRepository requests,
        IDateTime dateTime,
        IUnitOfWork unitOfWork,
        ILogger<ExpireOverdueGuideRequestsCommandHandler> logger)
    {
        _requests = requests;
        _dateTime = dateTime;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<int>> Handle(ExpireOverdueGuideRequestsCommand command, CancellationToken cancellationToken)
    {
        var batchSize = Math.Clamp(command.BatchSize, 1, GuideRequestPolicy.ExpirationBatchSize * 10);
        var now = _dateTime.UtcNow;

        // Only pending requests past ExpiresAt are returned, so accepted/rejected/cancelled/completed ones are never touched
        // and a request expired in an earlier run is not picked up again.
        var overdue = await _requests.ListExpiredPendingAsync(now, batchSize, cancellationToken);

        var expired = 0;
        foreach (var request in overdue)
        {
            var result = GuideDomainCall.Execute(() => request.Expire(now));
            if (result.IsFailure)
            {
                // A request that cannot be expired (state changed meanwhile) must not block the rest of the batch.
                _logger.LogWarning("Skipped expiring guide request {RequestId}: {Reason}", request.Id, result.Error.Description);
                continue;
            }

            expired++;
        }

        if (expired > 0)
        {
            // One save per batch. xmin concurrency on GuideRequest makes the whole save fail (and roll back) if a
            // request was accepted/cancelled after it was read; the worker simply retries on its next cycle.
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return expired;
    }
}
