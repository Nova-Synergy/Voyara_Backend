using FluentValidation;

namespace Voyara.Application.Trips.Commands.CreateTrip;

/// <summary>
/// Validates <see cref="CreateTripCommand"/>.
/// </summary>
public sealed class CreateTripCommandValidator : AbstractValidator<CreateTripCommand>
{
    public CreateTripCommandValidator()
    {
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
    }
}
