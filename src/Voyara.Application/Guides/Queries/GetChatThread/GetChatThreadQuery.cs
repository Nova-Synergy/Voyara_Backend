using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Guides.Queries.GetChatThread;

/// <summary>Chat thread header returned to a participant.</summary>
public sealed record ChatThreadDto(Guid Id, Guid GuideRequestId, bool IsClosed);

/// <summary>Gets a chat thread header; fails unless the current user is one of its two participants.</summary>
[AuthorizeRequest]
public sealed record GetChatThreadQuery(Guid ThreadId) : IQuery<ChatThreadDto>;
