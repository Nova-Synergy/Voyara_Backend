using FluentValidation;

namespace Voyara.Application.Planning.Commands.AddJourneyItem;

/// <summary>
/// Validator for <see cref="AddJourneyItemCommand"/>.
/// </summary>
public sealed class AddJourneyItemCommandValidator : AbstractValidator<AddJourneyItemCommand>
{
    /// <summary>
    /// Initializes validation rules for adding a destination stop to a journey.
    /// </summary>
    public AddJourneyItemCommandValidator()
    {
        RuleFor(x => x.JourneyId)
            .NotEmpty()
            .WithMessage("Journey ID is required.");

        RuleFor(x => x.DestinationId)
            .NotEmpty()
            .WithMessage("Destination ID is required.");

        RuleFor(x => x.DayNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Day number must be at least 1.");

        RuleFor(x => x.CityId)
            .GreaterThan(0)
            .When(x => x.CityId.HasValue)
            .WithMessage("City ID must be greater than zero.");

        RuleFor(x => x)
            .Must(x => !x.StartTime.HasValue || !x.EndTime.HasValue || x.EndTime.Value >= x.StartTime.Value)
            .WithMessage("End time must be greater than or equal to start time.");
    }
}
