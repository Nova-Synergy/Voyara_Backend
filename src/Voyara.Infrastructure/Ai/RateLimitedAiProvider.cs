using Voyara.Application.Abstractions;

namespace Voyara.Infrastructure.Ai;

/// <summary>Decorator limiting the number of concurrent AI requests with a semaphore.</summary>
public sealed class RateLimitedAiProvider : IAiProvider
{
    private readonly IAiProvider _inner;
    private readonly SemaphoreSlim _throttle;

    /// <summary>Creates the decorator.</summary>
    public RateLimitedAiProvider(IAiProvider inner, AiOptions options)
    {
        _inner = inner;
        _throttle = new SemaphoreSlim(Math.Max(1, options.MaxConcurrentRequests));
    }

    /// <inheritdoc />
    public async Task<string> GetCompletionAsync(string prompt, CancellationToken cancellationToken = default)
    {
        await _throttle.WaitAsync(cancellationToken);
        try
        {
            return await _inner.GetCompletionAsync(prompt, cancellationToken);
        }
        finally
        {
            _throttle.Release();
        }
    }
}
