using FluentValidation;
using Voyara.Application.DTOs.Guides;

namespace Voyara.Application.Guides;

public sealed class GuideLanguageInputValidator : AbstractValidator<GuideLanguageInput>
{
    public GuideLanguageInputValidator()
    {
        RuleFor(x => x.LanguageCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Proficiency).NotEmpty().MaximumLength(50);
    }
}

public sealed class GuideAvailabilityInputValidator : AbstractValidator<GuideAvailabilityInput>
{
    public GuideAvailabilityInputValidator()
    {
        RuleFor(x => x.Day).IsInEnum();
        RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime)
            .WithMessage("End time must be after start time.");
    }
}

public sealed class GuidePricingInputValidator : AbstractValidator<GuidePricingInput>
{
    public GuidePricingInputValidator()
    {
        RuleFor(x => x.Unit).IsInEnum();
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}
