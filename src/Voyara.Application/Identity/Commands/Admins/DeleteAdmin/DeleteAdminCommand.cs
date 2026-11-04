using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Identity.Commands.Admins.DeleteAdmin;

/// <summary>Soft-deletes an administrator account.</summary>
[AuthorizeRequest(Roles.Admin)]
public sealed record DeleteAdminCommand(Guid Id) : ICommand;
