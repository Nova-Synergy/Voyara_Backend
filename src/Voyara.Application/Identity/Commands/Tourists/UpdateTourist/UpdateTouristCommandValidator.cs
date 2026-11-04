using FluentValidation;

namespace Voyara.Application.Identity.Commands.Tourists.UpdateTourist;

public sealed class UpdateTouristCommandValidator : AbstractValidator<UpdateTouristCommand>
{
    public UpdateTouristCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NationalityCode).NotEmpty().Length(2);
        RuleFor(x => x.PreferredLanguage).NotEmpty().MaximumLength(10);
        RuleFor(x => x.PhotoUrl).MaximumLength(2048);
    }
}
