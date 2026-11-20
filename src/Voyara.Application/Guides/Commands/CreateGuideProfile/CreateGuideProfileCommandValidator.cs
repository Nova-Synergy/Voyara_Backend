using FluentValidation;

namespace Voyara.Application.Guides.Commands.CreateGuideProfile;

public sealed class CreateGuideProfileCommandValidator : AbstractValidator<CreateGuideProfileCommand>
{
    public CreateGuideProfileCommandValidator()
    {
        RuleFor(x => x.CityId).NotEmpty();
        RuleFor(x => x.Bio).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.PhotoUrl).MaximumLength(2048);
        RuleFor(x => x.YearsOfExperience).GreaterThanOrEqualTo(0);

        // CLS-04 multiplicity: at least one language and one price.
        RuleFor(x => x.Languages).NotNull().NotEmpty();
        RuleForEach(x => x.Languages).SetValidator(new GuideLanguageInputValidator());

        RuleFor(x => x.ExpertiseCategoryIds).NotNull();
        RuleForEach(x => x.ExpertiseCategoryIds).GreaterThan(0);

        RuleFor(x => x.Availability).NotNull();
        RuleForEach(x => x.Availability).SetValidator(new GuideAvailabilityInputValidator());

        RuleFor(x => x.Pricing).NotNull().NotEmpty();
        RuleForEach(x => x.Pricing).SetValidator(new GuidePricingInputValidator());
    }
}
