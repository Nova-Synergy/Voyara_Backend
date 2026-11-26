using FluentValidation;

namespace Voyara.Application.Identity.Commands.Logout;

/// <summary>Validates <see cref="LogoutCommand"/>.</summary>
public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    /// <summary>Creates the validator.</summary>
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty().MaximumLength(512);
    }
}
