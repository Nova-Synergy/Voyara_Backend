using RoleNames = Voyara.Application.Abstractions.Authorization.Roles;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Identity;

namespace Voyara.Application.Identity.Commands.Users.UpdateUser;

/// <summary>Updates a user's name, roles and active state.</summary>
[AuthorizeRequest(RoleNames.Admin)]
public sealed record UpdateUserCommand(
    Guid Id,
    string FullName,
    List<string>? Roles,
    bool? IsActive) : ICommand<UserReadDto>;
