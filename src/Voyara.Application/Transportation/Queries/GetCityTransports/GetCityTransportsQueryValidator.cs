using FluentValidation;

namespace Voyara.Application.Transportation.Queries.GetCityTransports;

/// <summary>Validates the listing filters. Paging values are normalized by the handler.</summary>
public sealed class GetCityTransportsQueryValidator : AbstractValidator<GetCityTransportsQuery>
{
    public GetCityTransportsQueryValidator()
    {
        RuleFor(x => x.TransportTypeId).GreaterThan(0).When(x => x.TransportTypeId.HasValue);
        RuleFor(x => x.MinCost).GreaterThanOrEqualTo(0m).When(x => x.MinCost.HasValue);
        RuleFor(x => x.MaxCost).GreaterThanOrEqualTo(0m).When(x => x.MaxCost.HasValue);
        RuleFor(x => x.MaxCost).GreaterThanOrEqualTo(x => x.MinCost!.Value)
            .When(x => x.MinCost.HasValue && x.MaxCost.HasValue)
            .WithMessage("Maximum cost cannot be lower than minimum cost.");
        RuleFor(x => x.Currency).Length(3).When(x => !string.IsNullOrWhiteSpace(x.Currency));
        RuleFor(x => x.Currency).NotEmpty()
            .When(x => x.MinCost.HasValue || x.MaxCost.HasValue)
            .WithMessage("Currency is required when filtering by cost.");
    }
}
