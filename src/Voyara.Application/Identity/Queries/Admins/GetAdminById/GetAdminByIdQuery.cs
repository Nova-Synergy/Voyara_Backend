using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Identity;

namespace Voyara.Application.Identity.Queries.Admins.GetAdminById;

/// <summary>Gets an administrator by identifier.</summary>
[AuthorizeRequest(Roles.Admin)]
public sealed record GetAdminByIdQuery(Guid Id) : IQuery<UserReadDto>;
