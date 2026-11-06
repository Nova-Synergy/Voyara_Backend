using FluentValidation;

namespace Voyara.Application.Catalog.Commands.CreateCategory;

public sealed class CreateCategoryCommandValidator
    : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Icon)
            .NotEmpty()
            .MaximumLength(200);
    }
}