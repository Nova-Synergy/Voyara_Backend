using FluentValidation;

namespace Voyara.Application.CostEstimation.Commands.GenerateCostEstimate;

/// <summary>
/// Validates <see cref="GenerateCostEstimateCommand"/>.
/// </summary>
public sealed class GenerateCostEstimateCommandValidator : AbstractValidator<GenerateCostEstimateCommand>
{
    public GenerateCostEstimateCommandValidator()
    {
        RuleFor(x => x.JourneyId)
            .NotEmpty()
            .WithMessage("Journey ID is required.");

        RuleFor(x => x.Currency)
            .MaximumLength(3)
            .WithMessage("Currency code must be a 3-character ISO code.")
            .When(x => !string.IsNullOrWhiteSpace(x.Currency));
    }
}
