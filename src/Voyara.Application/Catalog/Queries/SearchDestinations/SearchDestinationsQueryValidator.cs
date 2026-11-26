using System.Text.RegularExpressions;
using FluentValidation;
using Voyara.Domain.Repositories.Localization;

namespace Voyara.Application.Catalog.Queries.SearchDestinations;

/// <summary>Validates destination search requests.</summary>
public sealed class SearchDestinationsQueryValidator : AbstractValidator<SearchDestinationsQuery>
{
    /// <summary>Longest accepted search term.</summary>
    public const int MaxSearchTermLength = 200;

    /// <summary>Largest allowed page size.</summary>
    public const int MaxPageSize = 100;

    /// <summary>Largest allowed page number; keeps the row offset far from integer overflow.</summary>
    public const int MaxPage = 10_000;

    private static readonly Regex LanguageCodePattern =
        new("^[a-zA-Z]{2,3}(-[a-zA-Z0-9]{2,8})*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex CurrencyPattern =
        new("^[a-zA-Z]{3}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>Creates the validator.</summary>
    public SearchDestinationsQueryValidator(ILanguageRepository languageRepository)
    {
        RuleFor(query => query.SearchTerm)
            .MaximumLength(MaxSearchTermLength);

        // Rules run on the normalized (trimmed, lower-case) language, the same value the search uses.
        RuleFor(query => query.Language)
            .MaximumLength(10)
            .Must(code => LanguageCodePattern.IsMatch(code))
            .WithMessage("The language code is not well formed.")
            .When(query => !string.IsNullOrWhiteSpace(query.LanguageCode));

        // Only languages the application actually supports (rows of the languages table) are accepted. The resolved
        // language is checked, so the English default used when `lang` is omitted must exist as well (it is seeded by
        // LanguageConfiguration). This also bounds the number of distinct language values that can reach the cache.
        RuleFor(query => query.Language)
            .MustAsync((code, cancellationToken) => languageRepository.ExistsByCodeAsync(code, cancellationToken))
            .WithMessage(query => $"The language '{query.Language}' is not supported.")
            .When(query => IsWellFormedLanguage(query.Language));

        RuleFor(query => query.CountryId)
            .NotEqual(Guid.Empty)
            .When(query => query.CountryId.HasValue);

        RuleFor(query => query.CityId)
            .NotEqual(Guid.Empty)
            .When(query => query.CityId.HasValue);

        RuleFor(query => query.CategoryId)
            .NotEqual(Guid.Empty)
            .When(query => query.CategoryId.HasValue);

        RuleFor(query => query.MinRating)
            .InclusiveBetween(0d, 5d)
            .When(query => query.MinRating.HasValue);

        RuleFor(query => query.MaxPrice)
            .GreaterThanOrEqualTo(0m)
            .When(query => query.MaxPrice.HasValue);

        RuleFor(query => query.Currency)
            .NotEmpty()
            .WithMessage("A currency is required when a maximum price is given.")
            .When(query => query.MaxPrice.HasValue);

        RuleFor(query => query.Currency)
            .Must(currency => CurrencyPattern.IsMatch(currency!.Trim()))
            .WithMessage("Currency must be a 3-letter ISO 4217 code.")
            .When(query => !string.IsNullOrWhiteSpace(query.Currency));

        RuleFor(query => query.MaxPrice)
            .NotNull()
            .WithMessage("A maximum price is required when a currency is given.")
            .When(query => !string.IsNullOrWhiteSpace(query.Currency));

        RuleFor(query => query.SortBy)
            .IsInEnum();

        RuleFor(query => query.SortDirection)
            .IsInEnum()
            .When(query => query.SortDirection.HasValue);

        RuleFor(query => query.Page)
            .InclusiveBetween(1, MaxPage);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, MaxPageSize);
    }

    private static bool IsWellFormedLanguage(string code)
        => code.Length <= 10 && LanguageCodePattern.IsMatch(code);
}
