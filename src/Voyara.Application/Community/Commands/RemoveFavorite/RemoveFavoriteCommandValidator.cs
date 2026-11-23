
using FluentValidation;

namespace Voyara.Application.Community.Commands.RemoveFavorite;

/// <summary>
/// Validates <see cref="RemoveFavoriteCommand"/> requests before execution.
/// </summary>
public sealed class RemoveFavoriteCommandValidator
    : AbstractValidator<RemoveFavoriteCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveFavoriteCommandValidator"/> class
    /// and configures validation rules for removing a favorite destination.
    /// </summary>
    public RemoveFavoriteCommandValidator()
    {
        RuleFor(command => command.DestinationId)
            .NotEmpty()
            .WithMessage("Destination ID is required.");
    }
}
