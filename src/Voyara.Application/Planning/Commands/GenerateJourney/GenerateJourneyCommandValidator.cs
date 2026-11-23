using FluentValidation;

namespace Voyara.Application.Planning.Commands.GenerateJourney;

/// <summary>
/// Validator enforcing request constraints on <see cref="GenerateJourneyCommand"/>.
/// </summary>
public sealed class GenerateJourneyCommandValidator : AbstractValidator<GenerateJourneyCommand>
{
    /// <summary>
    /// Initializes validation rules for journey generation.
    /// </summary>
    public GenerateJourneyCommandValidator()
    {
        RuleFor(x => x.DurationDays)
            .InclusiveBetween(1, 30)
            .WithMessage("Duration must be between 1 and 30 days.");

        RuleFor(x => x.Title)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.BudgetAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.BudgetAmount.HasValue)
            .WithMessage("Budget amount must be non-negative.");

        RuleFor(x => x.BudgetCurrency)
            .MaximumLength(3)
            .When(x => !string.IsNullOrEmpty(x.BudgetCurrency));
    }
}
