using FluentValidation;

namespace Voyara.Application.Identity.Commands.TourGuides.RegisterTourGuide;

public sealed class RegisterTourGuideCommandValidator : AbstractValidator<RegisterTourGuideCommand>
{
    public RegisterTourGuideCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(128);
        RuleFor(x => x.CityId).GreaterThan(0);
        RuleFor(x => x.Bio).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.PhotoUrl).MaximumLength(2048);
        RuleFor(x => x.YearsOfExperience).InclusiveBetween(0, 80);
    }
}
