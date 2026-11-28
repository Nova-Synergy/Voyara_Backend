using Voyara.Application.Abstractions;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.AiAssistant.Services;

/// <summary>
/// Shared helper for executing AI completions, validating non-empty responses,
/// estimating token usage, and mapping provider failures to <see cref="Result"/>.
/// </summary>
internal static class AiResponseHelper
{
    public static async Task<Result<(string Content, int TokensUsed)>> ExecuteCompletionAsync(
        IAiProvider aiProvider,
        string prompt,
        CancellationToken cancellationToken)
    {
        string? completion;
        try
        {
            completion = await aiProvider.GetCompletionAsync(prompt, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            return Result.Failure<(string Content, int TokensUsed)>(
                Error.Failure("AiProvider.Unavailable", "The AI assistant could not generate a response right now."));
        }

        if (string.IsNullOrWhiteSpace(completion))
        {
            return Result.Failure<(string Content, int TokensUsed)>(
                Error.Failure("AiProvider.EmptyResponse", "The AI assistant returned an empty response."));
        }

        var trimmed = completion.Trim();
        var tokensUsed = Math.Max(1, (prompt.Length + trimmed.Length) / 4);
        return (trimmed, tokensUsed);
    }
}
