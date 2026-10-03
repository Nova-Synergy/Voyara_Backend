using FluentValidation;

namespace Voyara.Application.Catalog.Commands.CreateDestination;

/// <summary>Validates destination creation requests.</summary>
public sealed class CreateDestinationCommandValidator : AbstractValidator<CreateDestinationCommand>
{
    /// <summary>Creates the validator.</summary>
    public CreateDestinationCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(command => command.Description)
            .MaximumLength(4000);
        RuleFor(command => command.Latitude)
            .InclusiveBetween(-90d, 90d);
        RuleFor(command => command.Longitude)
            .InclusiveBetween(-180d, 180d);
    }
}
