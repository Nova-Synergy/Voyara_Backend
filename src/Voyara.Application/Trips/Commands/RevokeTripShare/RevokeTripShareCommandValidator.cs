using FluentValidation;

namespace Voyara.Application.Trips.Commands.RevokeTripShare;

/// <summary>
/// Validates <see cref="RevokeTripShareCommand"/>.
/// </summary>
public sealed class RevokeTripShareCommandValidator : AbstractValidator<RevokeTripShareCommand>
{
    public RevokeTripShareCommandValidator()
    {
        RuleFor(x => x.TripId)
            .NotEmpty()
            .WithMessage("Trip ID is required.");

        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Share token is required.");
    }
}
