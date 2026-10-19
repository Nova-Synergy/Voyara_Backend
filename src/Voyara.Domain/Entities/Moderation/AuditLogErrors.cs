using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Moderation;

/// <summary>Expected business errors of the <see cref="AuditLog"/> entity.</summary>
public static class AuditLogErrors
{
    /// <summary>The acting user is missing.</summary>
    public static readonly Error ActorRequired =
        Error.Validation("AuditLog.ActorRequired", "The acting user is required.");

    /// <summary>The action is empty.</summary>
    public static readonly Error ActionRequired =
        Error.Validation("AuditLog.ActionRequired", "The action is required.");

    /// <summary>The action exceeds the maximum length.</summary>
    public static readonly Error ActionTooLong =
        Error.Validation("AuditLog.ActionTooLong", $"The action can be at most {AuditLog.MaxActionLength} characters.");

    /// <summary>The target type is empty.</summary>
    public static readonly Error TargetTypeRequired =
        Error.Validation("AuditLog.TargetTypeRequired", "The target type is required.");

    /// <summary>The target type exceeds the maximum length.</summary>
    public static readonly Error TargetTypeTooLong =
        Error.Validation("AuditLog.TargetTypeTooLong",
            $"The target type can be at most {AuditLog.MaxTargetTypeLength} characters.");

    /// <summary>The target id is empty.</summary>
    public static readonly Error TargetIdRequired =
        Error.Validation("AuditLog.TargetIdRequired", "The target id is required.");

    /// <summary>The target id exceeds the maximum length.</summary>
    public static readonly Error TargetIdTooLong =
        Error.Validation("AuditLog.TargetIdTooLong",
            $"The target id can be at most {AuditLog.MaxTargetIdLength} characters.");
}