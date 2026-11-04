using RoleNames = Voyara.Application.Abstractions.Authorization.Roles;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Identity;

namespace Voyara.Application.Identity.Commands.Users.CreateUser;

/// <summary>Creates a user account. Roles default to Tourist when omitted.</summary>
[AuthorizeRequest(RoleNames.Admin)]
public sealed record CreateUserCommand(
    string FullName,
    string Email,
    string Password,
    List<string>? Roles) : ICommand<UserReadDto>;
