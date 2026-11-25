using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.AiAssistant.Commands.ArchiveConversation;

/// <summary>
/// Command to archive an active AI assistant conversation session.
/// </summary>
/// <param name="ConversationId">The conversation session identifier.</param>
public sealed record ArchiveConversationCommand(Guid ConversationId) : ICommand;
