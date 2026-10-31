using FluentValidation;
using Voyara.Domain.Entities.Culture_Rules;

namespace Voyara.Application.Culture_Rules.Commands.UpdateLocalRule;

/// <summary>Validates local rule update requests.</summary>
public sealed class UpdateLocalRuleCommandValidator : AbstractValidator<UpdateLocalRuleCommand>
{
    /// <summary>Creates the validator.</summary>
    public UpdateLocalRuleCommandValidator()
    {
        RuleFor(command => command.LocalRuleId)
            .NotEmpty();
        RuleFor(command => command.Type)
            .IsInEnum();
        RuleFor(command => command.Source)
            .NotEmpty()
            .When(command => command.IsOfficial)
            .WithMessage("An official rule must state its source.");
        RuleFor(command => command.Source)
            .MaximumLength(LocalRule.MaxSourceLength);
    }
}
