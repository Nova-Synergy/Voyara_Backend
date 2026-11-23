using FluentValidation;

namespace Voyara.Application.Community.Queries.GetPlaceSuggestionById;

/// <summary>
/// Validates <see cref="GetPlaceSuggestionByIdQuery"/> requests before execution.
/// </summary>
public sealed class GetPlaceSuggestionByIdQueryValidator
    : AbstractValidator<GetPlaceSuggestionByIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetPlaceSuggestionByIdQueryValidator"/> class
    /// and configures validation rules for retrieving a place suggestion by identifier.
    /// </summary>
    public GetPlaceSuggestionByIdQueryValidator()
    {
        RuleFor(query => query.SuggestionId)
            .NotEmpty();
    }
}