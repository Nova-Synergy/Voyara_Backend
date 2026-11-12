using Voyara.Domain.Enums;
using Voyara.Domain.Events.Moderation;
using Voyara.Domain.ValueObjects;
using Voyara.SharedKernel.Entities;
using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Moderation;

/// <summary>
/// Aggregate root representing a user's complaint about content or a person on the platform. The target is
/// polymorphic (<see cref="ReportTarget"/>) and referenced by id only, as is the reporter.
/// </summary>
/// <remarks>
/// Allowed status changes (state machine ST-07):
/// <list type="bullet">
/// <item><description>Reported: StartInvestigation gives Investigation.</description></item>
/// <item><description>Investigation: Resolve gives ActionTaken, Dismiss gives Resolved.</description></item>
/// <item><description>ActionTaken: RecordOutcome gives Resolved.</description></item>
/// <item><description>Resolved is final.</description></item>
/// </list>
/// A report cannot skip the investigation. Any other change returns a failed <see cref="Result"/> and leaves
/// the report untouched.
/// </remarks>
public sealed class Report : AggregateRoot
{
    /// <summary>Maximum length of the reason.</summary>
    public const int MaxReasonLength = 100;

    /// <summary>Maximum length of the optional comment.</summary>
    public const int MaxCommentLength = 2000;

    /// <summary>Identifier of the user who filed the report (reference to the Identity module).</summary>
    public Guid ReporterId { get; private set; }

    /// <summary>The reported object.</summary>
    public ReportTarget Target { get; private set; } = null!;

    /// <summary>The kind of object being reported.</summary>
    public ReportTargetType TargetType => Target.Type;

    /// <summary>The identifier of the reported object.</summary>
    public Guid TargetId => Target.Id;

    /// <summary>Short reason for the report.</summary>
    public string Reason { get; private set; } = string.Empty;

    /// <summary>Optional extra details from the reporter.</summary>
    public string? Comment { get; private set; }

    /// <summary>Current position in the report lifecycle.</summary>
    public ReportStatus Status { get; private set; } = ReportStatus.Reported;

    /// <summary>The action taken against the target; null until the report is upheld.</summary>
    public ModerationAction? ActionTaken { get; private set; }

    /// <summary>The admin who closed the report; null until it is resolved.</summary>
    public Guid? ResolvedBy { get; private set; }

    /// <summary>Whether the report has reached its final state.</summary>
    public bool IsResolved => Status == ReportStatus.Resolved;

    private Report()
    {
    }

    private Report(Guid reporterId, ReportTarget target, string reason, string? comment)
    {
        ReporterId = reporterId;
        Target = target;
        Reason = reason;
        Comment = comment;
        Status = ReportStatus.Reported;
    }

    /// <summary>
    /// Files a report in the Reported status after validating every rule, and raises
    /// <see cref="ReportFiledDomainEvent"/>. A user cannot report their own account. Duplicate reports by the
    /// same user are prevented by the Application layer (see <see cref="ReportErrors.AlreadyReported"/>).
    /// </summary>
    /// <returns>The new report, or the first rule that was broken.</returns>
    public static Result<Report> Create(
        Guid reporterId, ReportTargetType targetType, Guid targetId, string reason, string? comment = null)
    {
        if (reporterId == Guid.Empty)
        {
            return Result.Failure<Report>(ReportErrors.ReporterRequired);
        }

        var targetResult = ReportTarget.Create(targetType, targetId);
        if (targetResult.IsFailure)
        {
            return Result.Failure<Report>(targetResult.Error);
        }

        if (targetResult.Value.IsUser(reporterId))
        {
            return Result.Failure<Report>(ReportErrors.CannotReportYourself);
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            return Result.Failure<Report>(ReportErrors.ReasonRequired);
        }

        var trimmedReason = reason.Trim();
        if (trimmedReason.Length > MaxReasonLength)
        {
            return Result.Failure<Report>(ReportErrors.ReasonTooLong);
        }

        var trimmedComment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        if (trimmedComment is { Length: > MaxCommentLength })
        {
            return Result.Failure<Report>(ReportErrors.CommentTooLong);
        }

        var report = new Report(reporterId, targetResult.Value, trimmedReason, trimmedComment);

        report.RaiseDomainEvent(new ReportFiledDomainEvent(
            Guid.NewGuid(), DateTimeOffset.UtcNow, report.Id, reporterId, targetType, targetId, trimmedReason));

        return report;
    }

    /// <summary>Opens an investigation on a Reported report.</summary>
    /// <param name="adminId">The admin starting the investigation.</param>
    public Result StartInvestigation(Guid adminId)
    {
        if (adminId == Guid.Empty)
        {
            return Result.Failure(ReportErrors.AdminRequired);
        }

        if (Status != ReportStatus.Reported)
        {
            return Result.Failure(ReportErrors.InvalidTransition("start investigating", Status));
        }

        Status = ReportStatus.Investigation;

        RaiseDomainEvent(new ReportInvestigationStartedDomainEvent(
            Guid.NewGuid(), DateTimeOffset.UtcNow, Id, ReporterId, adminId));

        return Result.Success();
    }

    /// <summary>
    /// Upholds a report under investigation and records the action taken against the target. The action must
    /// make sense for the target: Hide and Delete for content, Warn and Suspend for users and guides.
    /// </summary>
    /// <param name="adminId">The admin resolving the report.</param>
    /// <param name="action">The moderation action taken.</param>
    public Result Resolve(Guid adminId, ModerationAction action)
    {
        if (adminId == Guid.Empty)
        {
            return Result.Failure(ReportErrors.AdminRequired);
        }

        if (!Enum.IsDefined(action))
        {
            return Result.Failure(ReportErrors.InvalidAction);
        }

        if (!Target.Allows(action))
        {
            return Result.Failure(ReportErrors.ActionNotApplicable(action, TargetType));
        }

        if (Status != ReportStatus.Investigation)
        {
            return Result.Failure(ReportErrors.InvalidTransition("take action on", Status));
        }

        Status = ReportStatus.ActionTaken;
        ActionTaken = action;

        RaiseDomainEvent(new ReportActionTakenDomainEvent(
            Guid.NewGuid(), DateTimeOffset.UtcNow, Id, TargetType, TargetId, adminId, action));

        return Result.Success();
    }

    /// <summary>
    /// Closes a report whose action has been applied ("outcome recorded"). The reporter is then notified and
    /// the audit entry is written, both driven by <see cref="ReportResolvedDomainEvent"/>.
    /// </summary>
    /// <param name="adminId">The admin closing the report.</param>
    public Result RecordOutcome(Guid adminId)
    {
        if (adminId == Guid.Empty)
        {
            return Result.Failure(ReportErrors.AdminRequired);
        }

        if (Status != ReportStatus.ActionTaken)
        {
            return Result.Failure(ReportErrors.InvalidTransition("record the outcome of", Status));
        }

        Status = ReportStatus.Resolved;
        ResolvedBy = adminId;

        RaiseDomainEvent(new ReportResolvedDomainEvent(
            Guid.NewGuid(), DateTimeOffset.UtcNow, Id, ReporterId, TargetType, TargetId, adminId, ActionTaken));

        return Result.Success();
    }

    /// <summary>
    /// Closes a report under investigation without any action because it was unfounded. The resolved event
    /// carries no action, which tells the owning module to restore the target.
    /// </summary>
    /// <param name="adminId">The admin dismissing the report.</param>
    public Result Dismiss(Guid adminId)
    {
        if (adminId == Guid.Empty)
        {
            return Result.Failure(ReportErrors.AdminRequired);
        }

        if (Status != ReportStatus.Investigation)
        {
            return Result.Failure(ReportErrors.InvalidTransition("dismiss", Status));
        }

        Status = ReportStatus.Resolved;
        ResolvedBy = adminId;

        RaiseDomainEvent(new ReportResolvedDomainEvent(
            Guid.NewGuid(), DateTimeOffset.UtcNow, Id, ReporterId, TargetType, TargetId, adminId, null));

        return Result.Success();
    }
}