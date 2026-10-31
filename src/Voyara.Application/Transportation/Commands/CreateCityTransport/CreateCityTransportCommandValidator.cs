using FluentValidation;
using Voyara.Domain.Entities.Transportation;

namespace Voyara.Application.Transportation.Commands.CreateCityTransport;

/// <summary>Validates city transport creation requests.</summary>
public sealed class CreateCityTransportCommandValidator : AbstractValidator<CreateCityTransportCommand>
{
    /// <summary>Creates the validator.</summary>
    public CreateCityTransportCommandValidator()
    {
        RuleFor(command => command.CityId)
            .GreaterThan(0);
        RuleFor(command => command.TransportTypeId)
            .GreaterThan(0);
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
