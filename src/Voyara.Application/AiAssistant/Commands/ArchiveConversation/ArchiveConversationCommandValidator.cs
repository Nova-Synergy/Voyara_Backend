using FluentValidation;

namespace Voyara.Application.AiAssistant.Commands.ArchiveConversation;

/// <summary>Validates conversation archive requests.</summary>
public sealed class ArchiveConversationCommandValidator : AbstractValidator<ArchiveConversationCommand>
{
    /// <summary>Creates the validator.</summary>
    public ArchiveConversationCommandValidator()
    {
        RuleFor(command => command.ConversationId)
            .NotEmpty();
    }
}
