using FluentValidation;

namespace Voyara.Application.Planning.Commands.MoveJourneyItem;

/// <summary>
/// Validator for <see cref="MoveJourneyItemCommand"/>.
/// </summary>
public sealed class MoveJourneyItemCommandValidator : AbstractValidator<MoveJourneyItemCommand>
{
    /// <summary>
    /// Initializes validation rules for moving an itinerary item.
    /// </summary>
    public MoveJourneyItemCommandValidator()
    {
        RuleFor(x => x.JourneyId)
            .NotEmpty()
            .WithMessage("Journey ID is required.");

        RuleFor(x => x.ItemId)
            .NotEmpty()
            .WithMessage("Item ID is required.");

        RuleFor(x => x.TargetDayNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Target day number must be at least 1.");

        RuleFor(x => x.NewOrder)
            .GreaterThanOrEqualTo(1)
            .WithMessage("New order must be at least 1.");
    }
}
