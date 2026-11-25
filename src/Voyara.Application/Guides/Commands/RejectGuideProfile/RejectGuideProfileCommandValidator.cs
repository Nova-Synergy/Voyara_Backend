using FluentValidation;

namespace Voyara.Application.Guides.Commands.RejectGuideProfile;

public sealed class RejectGuideProfileCommandValidator : AbstractValidator<RejectGuideProfileCommand>
{
    public RejectGuideProfileCommandValidator()
    {
        RuleFor(x => x.GuideProfileId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
