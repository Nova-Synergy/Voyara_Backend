using Microsoft.Extensions.Logging;
using Voyara.Application.Abstractions;

namespace Voyara.Infrastructure.Ai;

/// <summary>Decorator recording AI usage (prompt size) for observability and billing groundwork.</summary>
public sealed class UsageLoggingAiProvider : IAiProvider
{
    private readonly IAiProvider _inner;
    private readonly ILogger<UsageLoggingAiProvider> _logger;

    /// <summary>Creates the decorator.</summary>
    public UsageLoggingAiProvider(IAiProvider inner, ILogger<UsageLoggingAiProvider> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> GetCompletionAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var completion = await _inner.GetCompletionAsync(prompt, cancellationToken);
        _logger.LogInformation(
            "AI usage: prompt {PromptLength} chars, completion {CompletionLength} chars",
            prompt.Length,
            completion.Length);
        return completion;
    }
}
