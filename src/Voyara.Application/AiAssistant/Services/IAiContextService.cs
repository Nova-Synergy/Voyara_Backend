using Voyara.Application.DTOs;
using Voyara.Domain.Entities.AiAssistant;

namespace Voyara.Application.AiAssistant.Services;

/// <summary>
/// Assembles conversational AI context by combining session state, captured trip requirements,
/// message history, and relevant published catalog destinations.
/// </summary>
public interface IAiContextService
{
    /// <summary>
    /// Builds a structured context snapshot for the given conversation.
    /// </summary>
    Task<AiConversationContextDto> BuildContextAsync(
        Conversation conversation,
        string? searchHint = null,
        int maxDestinations = 5,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Formats a multi-turn LLM prompt enriched with trip requirements and catalog destinations.
    /// </summary>
    string FormatPrompt(
        Conversation conversation,
        AiConversationContextDto context,
        int recentMessageCount = 8);
}
