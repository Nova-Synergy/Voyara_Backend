namespace Voyara.Domain.Enums;

/// <summary>The kind of object a report is about.</summary>
public enum ReportTargetType
{
    /// <summary>A destination review.</summary>
    Review,

    /// <summary>A user account.</summary>
    User,

    /// <summary>A tour guide.</summary>
    Guide,

    /// <summary>A tourism destination.</summary>
    Destination,

    /// <summary>Any other content on the platform.</summary>
    Content
}