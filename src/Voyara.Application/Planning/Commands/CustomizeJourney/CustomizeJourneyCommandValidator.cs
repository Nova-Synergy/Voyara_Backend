using FluentValidation;

namespace Voyara.Application.Planning.Commands.CustomizeJourney;

/// <summary>
/// Validator for <see cref="CustomizeJourneyCommand"/>.
/// </summary>
public sealed class CustomizeJourneyCommandValidator : AbstractValidator<CustomizeJourneyCommand>
{
    /// <summary>
    /// Initializes validation rules for natural language journey customization.
    /// </summary>
    public CustomizeJourneyCommandValidator()
    {
        RuleFor(x => x.JourneyId)
            .NotEmpty()
            .WithMessage("Journey ID is required.");

        RuleFor(x => x.Instruction)
            .NotEmpty()
            .MaximumLength(1000)
            .WithMessage("Instruction cannot be empty and must not exceed 1000 characters.");
    }
}
