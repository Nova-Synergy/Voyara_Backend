using FluentValidation;

namespace Voyara.Application.Planning.Commands.RemoveJourneyItem;

/// <summary>
/// Validator for <see cref="RemoveJourneyItemCommand"/>.
/// </summary>
public sealed class RemoveJourneyItemCommandValidator : AbstractValidator<RemoveJourneyItemCommand>
{
    /// <summary>
    /// Initializes validation rules for removing a journey item.
    /// </summary>
    public RemoveJourneyItemCommandValidator()
    {
        RuleFor(x => x.JourneyId)
            .NotEmpty()
            .WithMessage("Journey ID is required.");

        RuleFor(x => x.ItemId)
            .NotEmpty()
            .WithMessage("Item ID is required.");
    }
}
