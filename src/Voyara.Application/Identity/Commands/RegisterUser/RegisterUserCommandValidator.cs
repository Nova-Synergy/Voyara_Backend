using FluentValidation;

namespace Voyara.Application.Identity.Commands.RegisterUser;

/// <summary>Validates <see cref="RegisterUserCommand"/>; mirrors the limits of the <c>User</c> domain model; the password rules are ASP.NET Core Identity's defaults.</summary>
public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    /// <summary>Longest accepted password; bounds the cost of hashing.</summary>
    public const int MaxPasswordLength = 128;

    /// <summary>Creates the validator.</summary>
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(254)
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(MaxPasswordLength)
            .Matches("[A-Z]").WithMessage("Password must contain an upper-case letter.")
            .Matches("[a-z]").WithMessage("Password must contain a lower-case letter.")
            .Matches("[0-9]").WithMessage("Password must contain a digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain a non-alphanumeric character.");
    }
}
