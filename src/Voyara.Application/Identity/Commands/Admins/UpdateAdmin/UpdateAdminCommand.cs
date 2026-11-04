using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Identity;

namespace Voyara.Application.Identity.Commands.Admins.UpdateAdmin;

/// <summary>Updates an administrator account.</summary>
[AuthorizeRequest(Roles.Admin)]
public sealed record UpdateAdminCommand(
    Guid Id,
    string FullName,
    bool? IsActive) : ICommand<UserReadDto>;
