using FluentValidation;

namespace Voyara.Application.Identity.Commands.UpdateMyTouristProfile;

/// <summary>Validates tourist profile update requests (mirrors the domain rules).</summary>
public sealed class UpdateMyTouristProfileCommandValidator : AbstractValidator<UpdateMyTouristProfileCommand>
{
    /// <summary>Creates the validator.</summary>
    public UpdateMyTouristProfileCommandValidator()
    {
        RuleFor(command => command.FullName)
            .NotEmpty()
            .Must(name => name is null || name.Trim().Length <= 300)
            .WithMessage("Full name cannot exceed 300 characters.");

        RuleFor(command => command.NationalityCode)
            .NotEmpty()
            .Must(code => code is null || IsTwoLetterCode(code.Trim()))
            .WithMessage("Nationality must be a two-letter country code.");
    }

    private static bool IsTwoLetterCode(string code)
        => code.Length == 2 && code.All(char.IsAsciiLetter);
}
