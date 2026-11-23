using FluentValidation;

namespace Voyara.Application.Identity.Commands.RefreshAccessToken;

/// <summary>Validates <see cref="RefreshAccessTokenCommand"/>.</summary>
public sealed class RefreshAccessTokenCommandValidator : AbstractValidator<RefreshAccessTokenCommand>
{
    /// <summary>Creates the validator.</summary>
    public RefreshAccessTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty().MaximumLength(512);
    }
}
