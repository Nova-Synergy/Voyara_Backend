namespace Voyara.Domain.Enums;

/// <summary>Decision state of a place suggestion.</summary>
public enum SuggestionStatus
{
    /// <summary>Waiting for an admin decision.</summary>
    Pending,

    /// <summary>Accepted by an admin; it can become a destination.</summary>
    Approved,

    /// <summary>Refused by an admin.</summary>
    Rejected
}