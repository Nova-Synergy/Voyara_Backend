using FluentValidation;

namespace Voyara.Application.Identity.Commands.UpdateMyPreferences;

/// <summary>Validates preference update requests (mirrors the domain rules).</summary>
public sealed class UpdateMyPreferencesCommandValidator : AbstractValidator<UpdateMyPreferencesCommand>
{
    private const int MaxInterests = 50;

    /// <summary>Creates the validator.</summary>
    public UpdateMyPreferencesCommandValidator()
    {
        RuleFor(command => command.PreferredLanguage)
            .NotEmpty()
            .Must(code => code is null || code.Trim().Length <= 10)
            .WithMessage("Language code cannot exceed 10 characters.");

        RuleFor(command => command.Budget)
            .IsInEnum();

        RuleFor(command => command.InterestCodes)
            .NotNull()
            .Must(codes => codes is null || codes.Count <= MaxInterests)
            .WithMessage($"At most {MaxInterests} interests can be selected.");

        RuleForEach(command => command.InterestCodes)
            .NotEmpty()
            .Must(code => code is null || code.Trim().Length <= 50)
            .WithMessage("Interest code cannot exceed 50 characters.");
    }
}
