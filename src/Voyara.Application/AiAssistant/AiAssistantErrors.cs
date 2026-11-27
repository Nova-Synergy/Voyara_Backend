using Voyara.SharedKernel.Results;

namespace Voyara.Application.AiAssistant;

/// <summary>Errors raised by the AI conversation use cases.</summary>
public static class AiAssistantErrors
{
    /// <summary>The request is authenticated but no user identifier could be read from the token.</summary>
    public static readonly Error UserNotAuthenticated =
        Error.Unauthorized("Conversation.UserNotAuthenticated", "The current user could not be identified.");

    /// <summary>The conversation does not exist, or it belongs to another user (existence is not disclosed).</summary>
    public static Error NotFound(Guid conversationId) =>
        Error.NotFound("Conversation.NotFound", $"Conversation '{conversationId}' was not found.");

    /// <summary>An archived conversation can no longer be completed.</summary>
    public static readonly Error Archived =
        Error.Conflict("Conversation.Archived", "The conversation is archived and cannot be completed.");
}
