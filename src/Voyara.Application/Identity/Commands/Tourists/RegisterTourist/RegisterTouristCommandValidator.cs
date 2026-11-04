using FluentValidation;

namespace Voyara.Application.Identity.Commands.Tourists.RegisterTourist;

public sealed class RegisterTouristCommandValidator : AbstractValidator<RegisterTouristCommand>
{
    public RegisterTouristCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(128);
        RuleFor(x => x.NationalityCode).NotEmpty().Length(2);
        RuleFor(x => x.PreferredLanguage).NotEmpty().MaximumLength(10);
        RuleFor(x => x.PhotoUrl).MaximumLength(2048);
    }
}
