using FluentValidation;

namespace Voyara.Application.Localization.Commands.CreateLanguage;

public sealed class CreateLanguageCommandValidator
    : AbstractValidator<CreateLanguageCommand>
{
    public CreateLanguageCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}