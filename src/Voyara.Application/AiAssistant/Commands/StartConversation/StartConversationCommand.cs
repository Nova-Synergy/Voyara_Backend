using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.AiAssistant.Commands.StartConversation;

/// <summary>A tourist starts a new conversation with the AI travel assistant.</summary>
/// <param name="Language">Language of the conversation (for example en or ar); null means English.</param>
[AuthorizeRequest(Roles.Tourist)]
public sealed record StartConversationCommand(string? Language) : ICommand<Guid>;
