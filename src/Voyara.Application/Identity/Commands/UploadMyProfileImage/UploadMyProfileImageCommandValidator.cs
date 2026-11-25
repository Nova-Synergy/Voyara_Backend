using FluentValidation;
using Voyara.Application.Identity.Common;

namespace Voyara.Application.Identity.Commands.UploadMyProfileImage;

/// <summary>Validates profile image uploads before the file is read.</summary>
public sealed class UploadMyProfileImageCommandValidator : AbstractValidator<UploadMyProfileImageCommand>
{
    /// <summary>Creates the validator.</summary>
    public UploadMyProfileImageCommandValidator()
    {
        RuleFor(command => command.Content)
            .NotNull();

        RuleFor(command => command.Length)
            .GreaterThan(0)
            .WithMessage("The image is empty.")
            .LessThanOrEqualTo(ProfileImageRules.MaxBytes)
            .WithMessage($"The image cannot exceed {ProfileImageRules.MaxBytes / (1024 * 1024)} MB.");

        RuleFor(command => command.ContentType)
            .Must(ProfileImageRules.IsAllowedContentType)
            .WithMessage("Only JPEG, PNG and WebP images are accepted.");
    }
}
