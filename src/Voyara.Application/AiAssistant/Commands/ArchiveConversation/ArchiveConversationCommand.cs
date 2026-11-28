using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.AiAssistant.Commands.ArchiveConversation;

/// <summary>
/// The owner archives a conversation, which hides it from the default conversation list. Archiving an archived
/// conversation is a no-op.
/// </summary>
/// <param name="ConversationId">The conversation to archive.</param>
[AuthorizeRequest(Roles.Tourist)]
public sealed record ArchiveConversationCommand(Guid ConversationId) : ICommand;
