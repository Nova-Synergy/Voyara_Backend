using FluentValidation;

namespace Voyara.Application.Guides.Queries.GetGuideReviews;

public sealed class GetGuideReviewsQueryValidator : AbstractValidator<GetGuideReviewsQuery>
{
    public GetGuideReviewsQueryValidator()
    {
        RuleFor(x => x.GuideProfileId).NotEmpty();
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, GuideRequestPolicy.MaxPageSize);
    }
}
