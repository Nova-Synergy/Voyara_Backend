using FluentValidation;

namespace Voyara.Application.Guides.Commands.CreateGuideRequest;

public sealed class CreateGuideRequestCommandValidator : AbstractValidator<CreateGuideRequestCommand>
{
    public CreateGuideRequestCommandValidator()
    {
        RuleFor(x => x.GuideProfileId).NotEmpty();
        RuleFor(x => x.DurationHours).InclusiveBetween(1, GuideRequestPolicy.MaxDurationHours);
        RuleFor(x => x.PartySize).GreaterThan(0);
        RuleFor(x => x.PreferredLanguage).NotEmpty().MaximumLength(10);
        RuleFor(x => x.SpecialRequirements).MaximumLength(1000);
    }
}
