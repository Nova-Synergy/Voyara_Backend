using FluentValidation;

namespace Voyara.Application.Trips.Commands.DeleteTrip;

/// <summary>
/// Validates <see cref="DeleteTripCommand"/>.
/// </summary>
public sealed class DeleteTripCommandValidator : AbstractValidator<DeleteTripCommand>
{
    public DeleteTripCommandValidator()
    {
        RuleFor(x => x.TripId)
            .NotEmpty()
            .WithMessage("Trip ID is required.");
    }
}
