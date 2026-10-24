using Voyara.Domain.Enums;
using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Moderation;

/// <summary>Expected business errors of the <see cref="Report"/> aggregate.</summary>
public static class ReportErrors
{
    /// <summary>The reporting user is missing.</summary>
    public static readonly Error ReporterRequired =
        Error.Validation("Report.ReporterRequired", "The reporting user is required.");

    /// <summary>A user tried to report their own account.</summary>
    public static readonly Error CannotReportYourself =
        Error.Validation("Report.CannotReportYourself", "You cannot report your own account.");

    /// <summary>The reason is empty.</summary>
    public static readonly Error ReasonRequired =
        Error.Validation("Report.ReasonRequired", "A reason is required.");

    /// <summary>The reason exceeds the maximum length.</summary>
    public static readonly Error ReasonTooLong =
        Error.Validation("Report.ReasonTooLong", $"The reason can be at most {Report.MaxReasonLength} characters.");

    /// <summary>The comment exceeds the maximum length.</summary>
    public static readonly Error CommentTooLong =
        Error.Validation("Report.CommentTooLong", $"The comment can be at most {Report.MaxCommentLength} characters.");

    /// <summary>The acting admin id is missing.</summary>
    public static readonly Error AdminRequired =
        Error.Validation("Report.AdminRequired", "The handling admin is required.");

    /// <summary>The moderation action is not a defined value.</summary>
    public static readonly Error InvalidAction =
        Error.Validation("Report.InvalidAction", "The moderation action is invalid.");

    /// <summary>
    /// The user already reported this target. A report cannot see other reports, so the Application layer
    /// checks for duplicates and returns this error (SEQ-12 answers with a conflict).
    /// </summary>
    public static readonly Error AlreadyReported =
        Error.Conflict("Report.AlreadyReported", "You have already reported this item.");

    /// <summary>The action cannot be applied to this kind of target.</summary>
    public static Error ActionNotApplicable(ModerationAction action, ReportTargetType targetType) =>
        Error.Validation("Report.ActionNotApplicable", $"The action {action} cannot be applied to a {targetType}.");

    /// <summary>The requested status change is not allowed from the current status.</summary>
    public static Error InvalidTransition(string operation, ReportStatus current) =>
        Error.Conflict("Report.InvalidTransition", $"Cannot {operation} a report that is {current}.");
}