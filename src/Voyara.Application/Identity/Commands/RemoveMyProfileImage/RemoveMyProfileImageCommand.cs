using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Identity.Commands.RemoveMyProfileImage;

/// <summary>Removes the profile image of the signed-in tourist; succeeds when there is none.</summary>
[AuthorizeRequest(Roles.Tourist)]
public sealed record RemoveMyProfileImageCommand : ICommand;
