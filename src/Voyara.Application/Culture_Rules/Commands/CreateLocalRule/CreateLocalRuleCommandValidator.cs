using FluentValidation;
using Voyara.Domain.Entities.Culture_Rules;

namespace Voyara.Application.Culture_Rules.Commands.CreateLocalRule;

/// <summary>Validates local rule creation requests.</summary>
public sealed class CreateLocalRuleCommandValidator : AbstractValidator<CreateLocalRuleCommand>
{
    /// <summary>Creates the validator.</summary>
    public CreateLocalRuleCommandValidator()
    {
        RuleFor(command => command.Type)
            .IsInEnum();
        RuleFor(command => command.ScopeType)
            .IsInEnum();
        RuleFor(command => command.ScopeTargetId)
            .NotEmpty()
            .Must((command, targetId) => RuleScopeParser.Parse(command.ScopeType, targetId).IsSuccess)
            .WithMessage("The scope target id does not match the scope type.");
        RuleFor(command => command.Source)
            .NotEmpty()
            .When(command => command.IsOfficial)
            .WithMessage("An official rule must state its source.");
        RuleFor(command => command.Source)
            .MaximumLength(LocalRule.MaxSourceLength);
        RuleFor(command => command.LanguageCode)
            .NotEmpty()
            .MaximumLength(LocalRuleTranslation.MaxLanguageCodeLength)
            .Matches("^[A-Za-z]{2,3}(-[A-Za-z0-9]{2,4})?$");
        RuleFor(command => command.Title)
            .NotEmpty()
            .MaximumLength(LocalRuleTranslation.MaxTitleLength);
        RuleFor(command => command.Content)
            .NotEmpty()
            .MaximumLength(LocalRuleTranslation.MaxContentLength);
    }
}
