using FluentValidation;

namespace Voyara.Application.Community.Commands.DeletePlaceSuggestion;

/// <summary>
/// Validates <see cref="DeletePlaceSuggestionCommand"/> requests before execution.
/// </summary>
public sealed class DeletePlaceSuggestionCommandValidator
    : AbstractValidator<DeletePlaceSuggestionCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeletePlaceSuggestionCommandValidator"/> class
    /// and configures validation rules for deleting a place suggestion.
    /// </summary>
    public DeletePlaceSuggestionCommandValidator()
    {
        RuleFor(command => command.SuggestionId)
            .NotEmpty();
    }
}