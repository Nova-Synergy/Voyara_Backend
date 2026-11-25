using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.AiAssistant.Queries.GetMyConversations;

/// <summary>
/// Query to list all conversation sessions created by the authenticated tourist.
/// </summary>
public sealed record GetMyConversationsQuery : IQuery<IReadOnlyList<ConversationSummaryResponse>>;
