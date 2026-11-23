using FluentValidation;

namespace Voyara.Application.Trips.Commands.UpdateTrip;

/// <summary>
/// Validates <see cref="UpdateTripCommand"/>.
/// </summary>
public sealed class UpdateTripCommandValidator : AbstractValidator<UpdateTripCommand>
{
    public UpdateTripCommandValidator()
    {
        RuleFor(x => x.TripId)
            .NotEmpty()
            .WithMessage("Trip ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Trip title is required.")
            .MaximumLength(200)
            .WithMessage("Trip title cannot exceed 200 characters.");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("Trip end date cannot precede start date.");

        RuleFor(x => x.Visibility)
            .IsInEnum()
            .WithMessage("A valid visibility level must be specified.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage("A valid trip status must be specified.");
    }
}
