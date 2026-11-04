using FluentValidation;

namespace Voyara.Application.Identity.Commands.TourGuides.UpdateTourGuide;

public sealed class UpdateTourGuideCommandValidator : AbstractValidator<UpdateTourGuideCommand>
{
    public UpdateTourGuideCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Bio).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.PhotoUrl).MaximumLength(2048);
        RuleFor(x => x.YearsOfExperience).InclusiveBetween(0, 80);
    }
}
