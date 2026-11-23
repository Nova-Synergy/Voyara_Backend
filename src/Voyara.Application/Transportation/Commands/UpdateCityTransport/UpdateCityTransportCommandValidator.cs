using FluentValidation;
using Voyara.Domain.Entities.Transportation;

namespace Voyara.Application.Transportation.Commands.UpdateCityTransport;

/// <summary>Input-shape validation; business invariants stay in the <c>CityTransport</c> aggregate.</summary>
public sealed class UpdateCityTransportCommandValidator : AbstractValidator<UpdateCityTransportCommand>
{
    public UpdateCityTransportCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(CityTransport.MaxTextLength);
        RuleFor(x => x.ApproxCost).GreaterThanOrEqualTo(0m);
        RuleFor(x => x.Currency).NotEmpty().Length(CityTransport.CurrencyCodeLength);
        RuleFor(x => x.OperatingHours).NotEmpty().MaximumLength(CityTransport.MaxOperatingHoursLength);
        RuleFor(x => x.SafetyInfo).MaximumLength(CityTransport.MaxTextLength);
    }
}
