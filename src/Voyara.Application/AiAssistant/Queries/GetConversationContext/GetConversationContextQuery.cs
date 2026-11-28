using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.AiAssistant.Queries.GetConversationContext;

/// <summary>
/// Query to retrieve the assembled AI planning context and matching catalog destinations for a conversation.
/// </summary>
/// <param name="ConversationId">The conversation identifier.</param>
/// <param name="SearchHint">Optional keyword to filter catalog context.</param>
/// <param name="MaxDestinations">Maximum number of catalog destinations to include.</param>
public sealed record GetConversationContextQuery(
    Guid ConversationId,
    string? SearchHint = null,
    int MaxDestinations = 5) : IQuery<AiConversationContextDto>;
