using FluentValidation;

namespace Voyara.Application.Catalog.Commands.ArchiveDestination;

/// <summary>Validates destination archiving requests.</summary>
public sealed class ArchiveDestinationCommandValidator : AbstractValidator<ArchiveDestinationCommand>
{
    /// <summary>Creates the validator.</summary>
    public ArchiveDestinationCommandValidator()
    {
        RuleFor(command => command.DestinationId)
            .NotEmpty();
    }
}
