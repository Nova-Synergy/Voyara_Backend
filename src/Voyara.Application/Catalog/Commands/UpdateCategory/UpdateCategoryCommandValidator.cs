using FluentValidation;

namespace Voyara.Application.Catalog.Commands.UpdateCategory;

public sealed class UpdateCategoryCommandValidator
    : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Icon)
            .NotEmpty()
            .MaximumLength(200);
    }
}