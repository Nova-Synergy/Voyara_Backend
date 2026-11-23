using FluentValidation;

namespace Voyara.Application.Community.Commands.CreatePlaceSuggestion;

/// <summary>
/// Validates <see cref="CreatePlaceSuggestionCommand"/> requests before execution.
/// </summary>
public sealed class CreatePlaceSuggestionCommandValidator
    : AbstractValidator<CreatePlaceSuggestionCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreatePlaceSuggestionCommandValidator"/> class
    /// and configures validation rules for creating a place suggestion.
    /// </summary>
    public CreatePlaceSuggestionCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Description)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(command => command.Latitude)
            .InclusiveBetween(-90, 90);

        RuleFor(command => command.Longitude)
            .InclusiveBetween(-180, 180);

        RuleFor(command => command.Reason)
            .NotEmpty()
            .MaximumLength(2000);
    }
}