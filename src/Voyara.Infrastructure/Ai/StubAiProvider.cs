using Voyara.Application.Abstractions;

namespace Voyara.Infrastructure.Ai;

/// <summary>Placeholder AI adapter returning a canned response; no real provider is integrated yet.</summary>
public sealed class StubAiProvider : IAiProvider
{
    /// <inheritdoc />
    public Task<string> GetCompletionAsync(string prompt, CancellationToken cancellationToken = default)
        => Task.FromResult("The AI travel assistant is not configured yet. [stub response]");
}
