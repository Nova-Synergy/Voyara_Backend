using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.AiAssistant.Queries.GetConversationById;

/// <summary>
/// Query to retrieve a conversation session and its complete message history by identifier.
/// </summary>
/// <param name="ConversationId">The conversation identifier.</param>
public sealed record GetConversationByIdQuery(Guid ConversationId) : IQuery<ConversationDetailResponse>;
