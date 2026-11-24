using FluentValidation;

namespace Voyara.Application.Community.Commands.AddFavorite;

/// <summary>
/// Validates requests to add a destination to the current user's favorites.
/// </summary>
public sealed class AddFavoriteCommandValidator
    : AbstractValidator<AddFavoriteCommand>
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="AddFavoriteCommandValidator"/> class.
    /// </summary>
    public AddFavoriteCommandValidator()
    {
        RuleFor(command => command.DestinationId)
            .NotEmpty()
            .WithMessage("Destination ID is required.");
    }
}
