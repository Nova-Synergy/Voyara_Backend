using System.Text.Json;
using MediatR;
using Voyara.Application.Abstractions.Events;
using Voyara.Domain.Entities.Moderation;
using Voyara.Domain.Events.Moderation;
using Voyara.Domain.Repositories.Moderation;

namespace Voyara.Application.Moderation.EventHandlers;

/// <summary>
/// Handles <see cref="ReportInvestigationStartedDomainEvent"/> notifications by recording
/// an <see cref="AuditLog"/> entry when an administrator starts investigating a report.
/// </summary>
public sealed class ReportInvestigationStartedEventHandler
    : INotificationHandler<DomainEventNotification<ReportInvestigationStartedDomainEvent>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportInvestigationStartedEventHandler"/> class.
    /// </summary>
    /// <param name="auditLogRepository">The repository used to persist audit-log entries.</param>
    public ReportInvestigationStartedEventHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    /// <summary>
    /// Processes the <see cref="DomainEventNotification{TDomainEvent}"/> wrapping a
    /// <see cref="ReportInvestigationStartedDomainEvent"/> and adds an audit-log entry to the repository.
    /// </summary>
    /// <param name="notification">The domain event notification containing the <see cref="ReportInvestigationStartedDomainEvent"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A completed <see cref="Task"/> once the audit-log entry has been added to the repository.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when creating the <see cref="AuditLog"/> entry fails.
    /// </exception>
    public Task Handle(
        DomainEventNotification<ReportInvestigationStartedDomainEvent> notification,
        CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var detailsJson = JsonSerializer.Serialize(new
        {
            domainEvent.EventId,
            domainEvent.ReporterId
        });

        var auditLogResult = AuditLog.Create(
            domainEvent.AdminId,
            "ReportInvestigationStarted",
            "Report",
            domainEvent.ReportId.ToString(),
            detailsJson,
            domainEvent.OccurredOn);

        if (auditLogResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Could not create the report investigation audit entry: {auditLogResult.Error}");
        }

        _auditLogRepository.Add(auditLogResult.Value);

        return Task.CompletedTask;
    }
}