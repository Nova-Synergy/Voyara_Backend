using FluentValidation;

namespace Voyara.Application.Catalog.Queries.SearchDestinations;

/// <summary>Validates destination search requests.</summary>
public sealed class SearchDestinationsQueryValidator : AbstractValidator<SearchDestinationsQuery>
{
    private const string LanguageCodePattern = "^[a-zA-Z]{2,3}(-[a-zA-Z0-9]{2,8})*$";

    /// <summary>Creates the validator.</summary>
    public SearchDestinationsQueryValidator()
    {
        RuleFor(query => query.SearchTerm)
            .MaximumLength(200);

        RuleFor(query => query.LanguageCode)
            .MaximumLength(10)
            .Matches(LanguageCodePattern)
            .When(query => !string.IsNullOrWhiteSpace(query.LanguageCode));

        RuleFor(query => query.CountryId)
            .GreaterThan(0)
            .When(query => query.CountryId.HasValue);

        RuleFor(query => query.CityId)
            .GreaterThan(0)
            .When(query => query.CityId.HasValue);

        RuleFor(query => query.CategoryId)
            .NotEqual(Guid.Empty)
            .When(query => query.CategoryId.HasValue);
    }
}
