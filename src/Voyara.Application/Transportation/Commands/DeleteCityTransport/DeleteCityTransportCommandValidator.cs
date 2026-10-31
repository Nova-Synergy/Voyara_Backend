using FluentValidation;

namespace Voyara.Application.Transportation.Commands.DeleteCityTransport;

/// <summary>Validates city transport deletion requests.</summary>
public sealed class DeleteCityTransportCommandValidator : AbstractValidator<DeleteCityTransportCommand>
{
    /// <summary>Creates the validator.</summary>
    public DeleteCityTransportCommandValidator()
    {
        RuleFor(command => command.CityTransportId)
            .NotEmpty();
    }
}
