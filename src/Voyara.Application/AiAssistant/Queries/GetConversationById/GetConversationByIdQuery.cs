using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.AiAssistant.Queries.GetConversationById;

/// <summary>
/// Gets one of the current user's conversations with its collected requirements and message history.
/// Not cached: the history changes with every message.
/// </summary>
/// <param name="ConversationId">The conversation to read.</param>
[AuthorizeRequest(Roles.Tourist)]
public sealed record GetConversationByIdQuery(Guid ConversationId) : IQuery<ConversationDetailsResponse>;
