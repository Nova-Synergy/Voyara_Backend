using FluentValidation;

namespace Voyara.Application.Catalog.Commands.PublishDestination;

/// <summary>Validates destination publication requests.</summary>
public sealed class PublishDestinationCommandValidator : AbstractValidator<PublishDestinationCommand>
{
    /// <summary>Creates the validator.</summary>
    public PublishDestinationCommandValidator()
    {
        RuleFor(command => command.DestinationId)
            .NotEmpty();
    }
}
