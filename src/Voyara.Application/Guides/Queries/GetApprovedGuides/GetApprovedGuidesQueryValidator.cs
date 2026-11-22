using FluentValidation;

namespace Voyara.Application.Guides.Queries.GetApprovedGuides;

public sealed class GetApprovedGuidesQueryValidator : AbstractValidator<GetApprovedGuidesQuery>
{
    public GetApprovedGuidesQueryValidator()
    {
        RuleFor(x => x.MinRating).InclusiveBetween(0m, 5m).When(x => x.MinRating.HasValue);
        RuleFor(x => x.MinPrice).GreaterThanOrEqualTo(0m).When(x => x.MinPrice.HasValue);
        RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(0m).When(x => x.MaxPrice.HasValue);
        RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(x => x.MinPrice!.Value)
            .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue)
            .WithMessage("Maximum price cannot be lower than minimum price.");
        RuleFor(x => x.PricingUnit).IsInEnum().When(x => x.PricingUnit.HasValue);
        RuleFor(x => x.Currency).Length(3).When(x => !string.IsNullOrWhiteSpace(x.Currency));
    }
}
