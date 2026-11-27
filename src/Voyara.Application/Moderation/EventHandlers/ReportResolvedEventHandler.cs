using System.Text.Json;
using MediatR;
using Voyara.Application.Abstractions.Events;
using Voyara.Domain.Entities.Moderation;
using Voyara.Domain.Events.Moderation;
using Voyara.Domain.Repositories.Moderation;

namespace Voyara.Application.Moderation.EventHandlers;

/// <summary>
/// Handles <see cref="ReportResolvedDomainEvent"/> notifications by recording
/// an <see cref="AuditLog"/> entry when an administrator resolves or dismisses a report.
/// </summary>
public sealed class ReportResolvedEventHandler
    : INotificationHandler<DomainEventNotification<ReportResolvedDomainEvent>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportResolvedEventHandler"/> class.
    /// </summary>
    /// <param name="auditLogRepository">The repository used to persist audit-log entries.</param>
    public ReportResolvedEventHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    /// <summary>
    /// Processes the <see cref="DomainEventNotification{TDomainEvent}"/> wrapping a
    /// <see cref="ReportResolvedDomainEvent"/> and adds a resolution or dismissal audit-log entry
    /// to the repository.
    /// </summary>
    /// <param name="notification">The domain event notification containing the <see cref="ReportResolvedDomainEvent"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A completed <see cref="Task"/> once the audit-log entry has been added to the repository.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when creating the <see cref="AuditLog"/> entry fails.
    /// </exception>
    public Task Handle(
        DomainEventNotification<ReportResolvedDomainEvent> notification,
        CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var detailsJson = JsonSerializer.Serialize(new
        {
            domainEvent.EventId,
            domainEvent.ReporterId,
            domainEvent.TargetType,
            domainEvent.TargetId,
            ActionTaken = domainEvent.ActionTaken?.ToString()
        });

        var action = domainEvent.ActionTaken.HasValue
            ? "ReportResolved"
            : "ReportDismissed";

        var auditLogResult = AuditLog.Create(
            domainEvent.AdminId,
            action,
            "Report",
            domainEvent.ReportId.ToString(),
            detailsJson,
            domainEvent.OccurredOn);

        if (auditLogResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Could not create the report resolution audit entry: {auditLogResult.Error}");
        }

        _auditLogRepository.Add(auditLogResult.Value);

        return Task.CompletedTask;
    }
}