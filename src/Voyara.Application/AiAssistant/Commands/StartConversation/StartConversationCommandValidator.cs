using FluentValidation;

namespace Voyara.Application.AiAssistant.Commands.StartConversation;

/// <summary>Validates conversation start requests.</summary>
public sealed class StartConversationCommandValidator : AbstractValidator<StartConversationCommand>
{
    private const string LanguageCodePattern = "^[a-zA-Z]{2,3}(-[a-zA-Z0-9]{2,8})*$";

    /// <summary>Creates the validator.</summary>
    public StartConversationCommandValidator()
    {
        RuleFor(command => command.Language)
            .MaximumLength(10)
            .Matches(LanguageCodePattern)
            .When(command => !string.IsNullOrWhiteSpace(command.Language));
    }
}
