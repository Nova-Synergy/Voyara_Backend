using System.Text.Json;
using MediatR;
using Voyara.Application.Abstractions.Events;
using Voyara.Domain.Entities.Moderation;
using Voyara.Domain.Events.Moderation;
using Voyara.Domain.Repositories.Moderation;

namespace Voyara.Application.Moderation.EventHandlers;

/// <summary>
/// Handles <see cref="ReportActionTakenDomainEvent"/> notifications by recording
/// an <see cref="AuditLog"/> entry when an administrator selects a moderation action.
/// </summary>
public sealed class ReportActionTakenEventHandler
    : INotificationHandler<DomainEventNotification<ReportActionTakenDomainEvent>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportActionTakenEventHandler"/> class.
    /// </summary>
    /// <param name="auditLogRepository">The repository used to persist audit-log entries.</param>
    public ReportActionTakenEventHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    /// <summary>
    /// Processes the <see cref="DomainEventNotification{TDomainEvent}"/> wrapping a
    /// <see cref="ReportActionTakenDomainEvent"/> and adds an audit-log entry to the repository.
    /// </summary>
    /// <param name="notification">The domain event notification containing the <see cref="ReportActionTakenDomainEvent"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A completed <see cref="Task"/> once the audit-log entry has been added to the repository.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when creating the <see cref="AuditLog"/> entry fails.
    /// </exception>
    public Task Handle(
        DomainEventNotification<ReportActionTakenDomainEvent> notification,
        CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var detailsJson = JsonSerializer.Serialize(new
        {
            domainEvent.EventId,
            domainEvent.ReportId,
            Action = domainEvent.Action.ToString()
        });

        var auditLogResult = AuditLog.Create(
            domainEvent.AdminId,
            "ReportActionTaken",
            domainEvent.TargetType.ToString(),
            domainEvent.TargetId.ToString(),
            detailsJson,
            domainEvent.OccurredOn);

        if (auditLogResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Could not create the moderation action audit entry: {auditLogResult.Error}");
        }

        _auditLogRepository.Add(auditLogResult.Value);

        return Task.CompletedTask;
    }
}