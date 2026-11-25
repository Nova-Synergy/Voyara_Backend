using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.AiAssistant.Commands.SendMessage;

/// <summary>
/// Command to send a user message to the AI travel assistant within an ongoing session.
/// </summary>
/// <param name="ConversationId">The conversation session identifier.</param>
/// <param name="Content">The textual prompt sent by the user.</param>
public sealed record SendMessageToAssistantCommand(
    Guid ConversationId,
    string Content) : ICommand<MessageResponse>;
