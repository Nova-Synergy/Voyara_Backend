using FluentValidation;
using Voyara.Application.Identity.Commands.RegisterUser;

namespace Voyara.Application.Identity.Commands.Login;

/// <summary>Validates the shape of <see cref="LoginCommand"/> only; credentials are checked by the handler.</summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    /// <summary>Creates the validator.</summary>
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().MaximumLength(254);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(RegisterUserCommandValidator.MaxPasswordLength);
    }
}
