using FluentValidation;

namespace Voyara.Application.Trips.Commands.ShareTrip;

/// <summary>
/// Validates <see cref="ShareTripCommand"/>.
/// </summary>
public sealed class ShareTripCommandValidator : AbstractValidator<ShareTripCommand>
{
    public ShareTripCommandValidator()
    {
        RuleFor(x => x.TripId)
            .NotEmpty()
            .WithMessage("Trip ID is required.");

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.ExpiresAt.HasValue)
            .WithMessage("Share expiration must be in the future.");
    }
}
