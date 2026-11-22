namespace Voyara.Application.Guides;

/// <summary>Application-level constants for the guide request workflow.</summary>
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
