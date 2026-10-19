namespace Voyara.Domain.Enums;

/// <summary>Moderation lifecycle of a review.</summary>
public enum ReviewStatus
{
    /// <summary>Submitted by the tourist and awaiting an admin decision.</summary>
    Pending,

    /// <summary>Accepted by an admin and publicly visible.</summary>
    Approved,

    /// <summary>Refused by an admin; the author is told why.</summary>
    Rejected,

    /// <summary>An approved review that users reported and that awaits an admin decision.</summary>
    Reported,

    /// <summary>Removed from public view by an admin.</summary>
    Hidden
}