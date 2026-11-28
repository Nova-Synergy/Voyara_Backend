using FluentValidation;

namespace Voyara.Application.AiAssistant.Commands.SendMessage;

/// <summary>
/// Validates AI assistant message submission commands.
/// </summary>
public sealed class SendMessageToAssistantCommandValidator : AbstractValidator<SendMessageToAssistantCommand>
{
    /// <summary>
    /// Initializes a new instance of <see cref="SendMessageToAssistantCommandValidator"/>.
    /// </summary>
    public SendMessageToAssistantCommandValidator()
    {
        RuleFor(command => command.ConversationId)
            .NotEmpty();

        RuleFor(command => command.Content)
            .NotEmpty()
            .MaximumLength(4000);
    }
}
