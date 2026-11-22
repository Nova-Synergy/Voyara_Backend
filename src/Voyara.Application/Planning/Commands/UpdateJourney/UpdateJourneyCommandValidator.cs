using FluentValidation;
using Voyara.Domain.Enums;

namespace Voyara.Application.Planning.Commands.UpdateJourney;

/// <summary>
/// Validator for <see cref="UpdateJourneyCommand"/>.
/// </summary>
public sealed class UpdateJourneyCommandValidator : AbstractValidator<UpdateJourneyCommand>
{
    /// <summary>
    /// Initializes validation rules for updating journey metadata.
    /// </summary>
    public UpdateJourneyCommandValidator()
    {
        RuleFor(x => x.JourneyId)
            .NotEmpty()
            .WithMessage("Journey ID is required.");

        RuleFor(x => x.Title)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Status)
            .Must(status => string.IsNullOrEmpty(status) || Enum.TryParse<JourneyStatus>(status, true, out _))
            .WithMessage("Status must be a valid JourneyStatus value (Draft, Confirmed, InProgress, Completed, Cancelled).");
    }
}
