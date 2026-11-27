using FluentValidation;

namespace Voyara.Application.Guides.Commands.CreateGuideReview;

public sealed class CreateGuideReviewCommandValidator : AbstractValidator<CreateGuideReviewCommand>
{
    public CreateGuideReviewCommandValidator()
    {
        RuleFor(x => x.GuideRequestId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Text).NotEmpty().MaximumLength(2000);
    }
}
