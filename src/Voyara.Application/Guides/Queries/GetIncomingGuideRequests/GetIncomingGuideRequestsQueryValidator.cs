using FluentValidation;

namespace Voyara.Application.Guides.Queries.GetIncomingGuideRequests;

public sealed class GetIncomingGuideRequestsQueryValidator : AbstractValidator<GetIncomingGuideRequestsQuery>
{
    public GetIncomingGuideRequestsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, GuideRequestPolicy.MaxPageSize);
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
    }
}
