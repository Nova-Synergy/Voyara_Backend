using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Guides;

namespace Voyara.Application.Guides.Commands.SendChatMessage;

/// <summary>Posts a message to a chat thread. The sender is always the authenticated user. Used by both REST and SignalR.</summary>
[AuthorizeRequest]
public sealed record SendChatMessageCommand(Guid ThreadId, string Text) : ICommand<ChatMessageDto>;
