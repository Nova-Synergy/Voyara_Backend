using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.AiAssistant.Commands.CompleteConversation;

/// <summary>The owner marks a conversation as finished. Completing a completed conversation is a no-op.</summary>
/// <param name="ConversationId">The conversation to complete.</param>
[AuthorizeRequest(Roles.Tourist)]
public sealed record CompleteConversationCommand(Guid ConversationId) : ICommand;
