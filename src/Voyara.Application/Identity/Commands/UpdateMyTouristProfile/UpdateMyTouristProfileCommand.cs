using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Identity.Commands.UpdateMyTouristProfile;

/// <summary>Updates the personal details of the signed-in tourist: display name and nationality.</summary>
[AuthorizeRequest(Roles.Tourist)]
public sealed record UpdateMyTouristProfileCommand(string FullName, string NationalityCode)
    : ICommand<TouristProfileResponse>;
