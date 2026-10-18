using Voyara.Domain.Enums;
using Voyara.SharedKernel.Events;

namespace Voyara.Domain.Events.Moderation;

/// <summary>
/// Raised when a user files a report. Lets the owning module flag the target (for example Community marking
/// a review as reported) and puts the report in the admins' queue.
/// </summary>
public sealed record ReportFiledDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid ReportId,
    Guid ReporterId,
    ReportTargetType TargetType,
    Guid TargetId,
    string Reason) : IDomainEvent;

/// <summary>Raised when an admin opens an investigation. Recorded in the audit trail.</summary>
public sealed record ReportInvestigationStartedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid ReportId,
    Guid ReporterId,
    Guid AdminId) : IDomainEvent;

/// <summary>
/// Raised when an admin upholds a report and chooses an action. The owning module of the target reacts by
/// applying it (for example hiding a review or suspending a user).
/// </summary>
public sealed record ReportActionTakenDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid ReportId,
    ReportTargetType TargetType,
    Guid TargetId,
    Guid AdminId,
    ModerationAction Action) : IDomainEvent;

/// <summary>
/// Raised when a report reaches its final state, whether it was dismissed or its outcome was recorded.
/// <paramref name="ActionTaken"/> is null when the report was dismissed (the owning module then restores the
/// target, for example a review back to Approved). Used to notify the reporter and to write the audit entry.
/// </summary>
public sealed record ReportResolvedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid ReportId,
    Guid ReporterId,
    ReportTargetType TargetType,
    Guid TargetId,
    Guid AdminId,
    ModerationAction? ActionTaken) : IDomainEvent;