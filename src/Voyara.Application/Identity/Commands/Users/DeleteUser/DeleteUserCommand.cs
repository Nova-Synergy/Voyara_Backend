using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Identity.Commands.Users.DeleteUser;

/// <summary>Soft-deletes a user and revokes their sessions.</summary>
[AuthorizeRequest(Roles.Admin)]
public sealed record DeleteUserCommand(Guid Id) : ICommand;
