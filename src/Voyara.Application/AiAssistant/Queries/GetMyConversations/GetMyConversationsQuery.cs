using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.AiAssistant.Queries.GetMyConversations;

/// <summary>
/// Query to list conversation sessions created by the authenticated tourist with pagination.
/// </summary>
/// <param name="Page">1-based page number.</param>
/// <param name="PageSize">Number of conversations per page.</param>
public sealed record GetMyConversationsQuery(
    int Page = 1,
    int PageSize = 10) : IQuery<PagedResult<ConversationSummaryResponse>>;
