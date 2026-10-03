namespace Voyara.Application.Abstractions;

/// <summary>Adapter contract for AI completion services (e.g. the travel assistant).</summary>
public interface IAiProvider
{
    /// <summary>Returns the assistant's completion for the given prompt.</summary>
    Task<string> GetCompletionAsync(string prompt, CancellationToken cancellationToken = default);
}
