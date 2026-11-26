using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Guides;

namespace Voyara.Application.Guides.Queries.GetChatMessages;

/// <summary>Lists a thread's messages oldest first; allowed for the two participants only.</summary>
[AuthorizeRequest]
public sealed record GetChatMessagesQuery(Guid ThreadId, int Page = 1, int PageSize = 50)
    : IQuery<PagedResult<ChatMessageDto>>;
