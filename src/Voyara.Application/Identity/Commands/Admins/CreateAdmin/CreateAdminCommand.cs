using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Identity;

namespace Voyara.Application.Identity.Commands.Admins.CreateAdmin;

/// <summary>Creates an administrator account.</summary>
[AuthorizeRequest(Roles.Admin)]
public sealed record CreateAdminCommand(
    string FullName,
    string Email,
    string Password) : ICommand<UserReadDto>;
