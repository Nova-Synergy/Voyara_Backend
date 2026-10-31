using FluentValidation;

namespace Voyara.Application.Culture_Rules.Commands.DeleteLocalRule;

/// <summary>Validates local rule deletion requests.</summary>
public sealed class DeleteLocalRuleCommandValidator : AbstractValidator<DeleteLocalRuleCommand>
{
    /// <summary>Creates the validator.</summary>
    public DeleteLocalRuleCommandValidator()
    {
        RuleFor(command => command.LocalRuleId)
            .NotEmpty();
    }
}
