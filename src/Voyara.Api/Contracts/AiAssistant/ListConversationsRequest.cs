using Voyara.Domain.Enums;

namespace Voyara.Api.Contracts.AiAssistant;

/// <summary>Query-string parameters of the conversation list endpoint.</summary>
public sealed record ListConversationsRequest
{
    /// <summary>Only conversations in this state; omitted lists every state except Archived.</summary>
    public ConversationState? State { get; init; }

    /// <summary>One-based page number.</summary>
    public int Page { get; init; } = 1;

    /// <summary>Items per page (1 to 100).</summary>
    public int PageSize { get; init; } = 20;
}
