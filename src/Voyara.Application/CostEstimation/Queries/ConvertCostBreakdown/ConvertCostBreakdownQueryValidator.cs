using FluentValidation;

namespace Voyara.Application.CostEstimation.Queries.ConvertCostBreakdown;

/// <summary>
/// Validates <see cref="ConvertCostBreakdownQuery"/>.
/// </summary>
public sealed class ConvertCostBreakdownQueryValidator : AbstractValidator<ConvertCostBreakdownQuery>
{
    public ConvertCostBreakdownQueryValidator()
    {
        RuleFor(x => x.JourneyId)
            .NotEmpty()
            .WithMessage("Journey ID is required.");

        RuleFor(x => x.TargetCurrency)
            .NotEmpty()
            .WithMessage("Target currency is required.")
            .Length(3)
            .WithMessage("Target currency must be a 3-character ISO code.");
    }
}
