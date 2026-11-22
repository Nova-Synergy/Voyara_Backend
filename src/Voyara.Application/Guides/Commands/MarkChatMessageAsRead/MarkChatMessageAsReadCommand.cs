using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Guides;

namespace Voyara.Application.Guides.Commands.MarkChatMessageAsRead;

/// <summary>Marks a message sent by the other participant as read.</summary>
[AuthorizeRequest]
public sealed record MarkChatMessageAsReadCommand(Guid ThreadId, Guid MessageId) : ICommand<ChatMessageDto>;
