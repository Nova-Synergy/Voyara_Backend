using FluentValidation;
using Voyara.Domain.Entities.Transportation;

namespace Voyara.Application.Transportation.Commands.UpdateCityTransport;

/// <summary>Validates city transport update requests.</summary>
public sealed class UpdateCityTransportCommandValidator : AbstractValidator<UpdateCityTransportCommand>
{
    /// <summary>Creates the validator.</summary>
    public UpdateCityTransportCommandValidator()
    {
        RuleFor(command => command.CityTransportId)
            .NotEmpty();
        RuleFor(command => command.Description)
            .NotEmpty()
            .MaximumLength(CityTransport.MaxTextLength);
        RuleFor(command => command.ApproxCost)
            .GreaterThanOrEqualTo(0m);
        RuleFor(command => command.Currency)
            .NotEmpty()
            .Length(CityTransport.CurrencyCodeLength)
            .Matches("^[A-Za-z]{3}$");
        RuleFor(command => command.OperatingHours)
            .NotEmpty()
            .MaximumLength(CityTransport.MaxOperatingHoursLength);
        RuleFor(command => command.SafetyInfo)
            .MaximumLength(CityTransport.MaxTextLength);
    }
}
