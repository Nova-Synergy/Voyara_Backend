using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Identity.Queries.GetCurrentUser;

/// <summary>Returns the profile of the authenticated caller (any role).</summary>
[AuthorizeRequest]
public sealed record GetCurrentUserQuery : IQuery<CurrentUserResponse>;
