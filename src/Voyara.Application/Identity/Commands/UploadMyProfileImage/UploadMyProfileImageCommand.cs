using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Identity.Commands.UploadMyProfileImage;

/// <summary>
/// Sets or replaces the profile image of the signed-in tourist; returns the stored photo reference.
/// The stream stays owned by the caller.
/// </summary>
/// <param name="Content">The image bytes.</param>
/// <param name="ContentType">The content type declared by the client (image/jpeg, image/png or image/webp).</param>
/// <param name="Length">The declared size in bytes.</param>
[AuthorizeRequest(Roles.Tourist)]
public sealed record UploadMyProfileImageCommand(Stream Content, string ContentType, long Length)
    : ICommand<string>;
