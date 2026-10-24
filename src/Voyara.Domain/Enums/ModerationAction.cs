namespace Voyara.Domain.Enums;

/// <summary>The action an admin takes against the target of an upheld report (SEQ-12).</summary>
public enum ModerationAction
{
    /// <summary>Remove the content from public view but keep it for audit.</summary>
    Hide,

    /// <summary>Delete (archive) the content.</summary>
    Delete,

    /// <summary>Warn the user or guide.</summary>
    Warn,

    /// <summary>Suspend the user or guide.</summary>
    Suspend
}