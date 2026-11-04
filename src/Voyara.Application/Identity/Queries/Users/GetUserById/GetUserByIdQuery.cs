using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Identity;

namespace Voyara.Application.Identity.Queries.Users.GetUserById;

/// <summary>Gets a user by identifier.</summary>
[AuthorizeRequest(Roles.Admin)]
public sealed record GetUserByIdQuery(Guid Id) : IQuery<UserReadDto>;
