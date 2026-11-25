using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Identity.Queries.GetMyProfileImage;

/// <summary>Opens the stored profile image of the signed-in tourist; the caller disposes the stream.</summary>
[AuthorizeRequest(Roles.Tourist)]
public sealed record GetMyProfileImageQuery : IQuery<ProfileImageResponse>;
