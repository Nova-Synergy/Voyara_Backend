namespace Voyara.Infrastructure.Ai;

/// <summary>AI adapter settings.</summary>
public sealed class AiOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Ai";

    /// <summary>Maximum number of in-flight AI requests enforced by the rate-limiting decorator.</summary>
    public int MaxConcurrentRequests { get; set; } = 4;
}
