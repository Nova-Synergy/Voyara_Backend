using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;
using Voyara.Domain.Enums;

namespace Voyara.Application.AiAssistant.Queries.GetMyConversations;

/// <summary>
/// Lists the current user's conversations, newest first. Archived conversations are hidden unless the Archived
/// state is requested explicitly. Not cached: the list changes with every message.
/// </summary>
/// <param name="State">Only conversations in this state; null lists every state except Archived.</param>
/// <param name="Page">One-based page number.</param>
/// <param name="PageSize">Items per page (1 to 100).</param>
[AuthorizeRequest(Roles.Tourist)]
public sealed record GetMyConversationsQuery(ConversationState? State, int Page, int PageSize)
    : IQuery<PagedResult<ConversationSummaryResponse>>;
