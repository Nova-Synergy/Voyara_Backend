namespace Voyara.Domain.Enums;

/// <summary>
/// Defines the possible statuses of a guide profile.
/// </summary>
public enum GuideStatus
{
    /// <summary>
    /// The guide profile is awaiting administrator approval.
    /// </summary>
    Pending,

    /// <summary>
    /// The guide profile has been approved and is active.
    /// </summary>
    Approved,

    /// <summary>
    /// The guide profile has been rejected by an administrator.
    /// </summary>
    Rejected,

    /// <summary>
    /// The guide profile has been rejected by an administrator.
    /// </summary>
    Suspended
}
