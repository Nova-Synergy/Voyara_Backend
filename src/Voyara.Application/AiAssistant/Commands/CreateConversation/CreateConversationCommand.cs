using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.AiAssistant.Commands.CreateConversation;

/// <summary>
/// Command to initiate a new planning conversation with the AI travel assistant.
/// </summary>
/// <param name="Language">Preferred interaction language code (defaults to "en").</param>
public sealed record CreateConversationCommand(string Language = "en") : ICommand<ConversationDetailResponse>;
