using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Identity.Queries.GetMyTouristProfile;

/// <summary>Gets the profile and preferences of the signed-in tourist.</summary>
[AuthorizeRequest(Roles.Tourist)]
public sealed record GetMyTouristProfileQuery : IQuery<TouristProfileResponse>;
