namespace Voyara.Application.Guides;

/// <summary>
/// Application-level constants for the guide request workflow.
/// <para>
/// <b>Time policy:</b> a request's <c>Date</c>/<c>StartTime</c> and the guide's availability windows and time-off dates are
/// wall-clock values in the <i>guide's</i> time zone (the IANA zone of the guide's city's country). "Is this in the past?"
/// is evaluated against the current time in that zone. Instants (<c>CreatedAt</c>, <c>ExpiresAt</c>, message times) are UTC.
/// </para>
/// </summary>
public static class GuideRequestPolicy
{
    /// <summary>How long a guide has to answer a pending request before it expires.</summary>
    public static readonly TimeSpan ResponseWindow = TimeSpan.FromHours(24);

    /// <summary>Longest tour that can be requested (a tour cannot span midnight).</summary>
    public const int MaxDurationHours = 24;

    /// <summary>Largest page size accepted by paged queries.</summary>
    public const int MaxPageSize = 100;

    /// <summary>Number of requests the expiration job processes per batch.</summary>
    public const int ExpirationBatchSize = 100;
}
