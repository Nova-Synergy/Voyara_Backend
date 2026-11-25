namespace Voyara.Infrastructure.Ai;

/// <summary>AI adapter settings.</summary>
public sealed class AiOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Ai";

    /// <summary>Maximum number of in-flight AI requests enforced by the rate-limiting decorator.</summary>
    public int MaxConcurrentRequests { get; set; } = 4;

    /// <summary>Base URL for the local Ollama instance (default: http://localhost:11434).</summary>
    public string OllamaBaseUrl { get; set; } = "http://localhost:11434";

    /// <summary>Ollama model identifier (default: qwen3.5:4b).</summary>
    public string Model { get; set; } = "qwen3.5:4b";

    /// <summary>Timeout in seconds for model generation requests.</summary>
    public int TimeoutSeconds { get; set; } = 120;
}
