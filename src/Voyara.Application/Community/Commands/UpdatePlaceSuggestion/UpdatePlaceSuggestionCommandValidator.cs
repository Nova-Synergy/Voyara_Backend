using FluentValidation;

namespace Voyara.Application.Community.Commands.UpdatePlaceSuggestion;

/// <summary>
/// Validates <see cref="UpdatePlaceSuggestionCommand"/> requests before execution.
/// </summary>
public sealed class UpdatePlaceSuggestionCommandValidator
    : AbstractValidator<UpdatePlaceSuggestionCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdatePlaceSuggestionCommandValidator"/> class
    /// and configures validation rules for updating a place suggestion.
    /// </summary>
    public UpdatePlaceSuggestionCommandValidator()
    {
        RuleFor(command => command.SuggestionId)
            .NotEmpty();

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