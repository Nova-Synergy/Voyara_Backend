using FluentValidation;

namespace Voyara.Application.Guides.Queries.GetMyGuideRequests;

public sealed class GetMyGuideRequestsQueryValidator : AbstractValidator<GetMyGuideRequestsQuery>
{
    public GetMyGuideRequestsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, GuideRequestPolicy.MaxPageSize);
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
    }
}
