using FluentValidation;

namespace Voyara.Application.AiAssistant.Commands.CompleteConversation;

/// <summary>Validates conversation completion requests.</summary>
public sealed class CompleteConversationCommandValidator : AbstractValidator<CompleteConversationCommand>
{
    /// <summary>Creates the validator.</summary>
    public CompleteConversationCommandValidator()
    {
        RuleFor(command => command.ConversationId)
            .NotEmpty();
    }
}
