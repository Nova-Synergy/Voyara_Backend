namespace Voyara.Domain.Enums;

/// <summary>Lifecycle of a user report.</summary>
public enum ReportStatus
{
    /// <summary>Filed by a user; in the moderation queue and the target is flagged.</summary>
    Reported,

    /// <summary>An admin has opened an investigation.</summary>
    Investigation,

    /// <summary>The report was upheld and a moderation action was taken against the target.</summary>
    ActionTaken,

    /// <summary>The report is closed: either dismissed, or its outcome was recorded.</summary>
    Resolved
}