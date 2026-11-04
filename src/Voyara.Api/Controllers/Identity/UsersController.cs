using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Authorization;
using Voyara.Application.DTOs.Identity;
using Voyara.Application.Identity.Commands.Users.CreateUser;
using Voyara.Application.Identity.Commands.Users.DeleteUser;
using Voyara.Application.Identity.Commands.Users.UpdateUser;
using Voyara.Application.Identity.Queries.Users.GetUserById;
using Voyara.Application.Identity.Queries.Users.SearchUsers;

namespace Voyara.Api.Controllers.Identity;

/// <summary>Admin CRUD over every user account, whatever its role.</summary>
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/v1/admin/users")]
public sealed class UsersController : IdentityControllerBase
{
    /// <summary>Searches users by name or e-mail, optionally by role, with paging.</summary>
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm, [FromQuery] string? role,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(
            new SearchUsersQuery(searchTerm, role, page, pageSize), cancellationToken));

    /// <summary>Gets a user by identifier.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new GetUserByIdQuery(id), cancellationToken));

    /// <summary>Creates a user; the role defaults to Tourist.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto dto, CancellationToken cancellationToken)
        => HandleCreated(
            await Sender.Send(
                new CreateUserCommand(dto.FullName, dto.Email, dto.Password, dto.Roles),
                cancellationToken),
            nameof(GetById),
            u => new { id = u.Id });

    /// <summary>Updates a user's name, roles and active state.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateUserDto dto, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new UpdateUserCommand(id, dto.FullName, dto.Roles, dto.IsActive), cancellationToken));

    /// <summary>Soft-deletes a user and revokes their sessions.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => HandleNoContent(await Sender.Send(new DeleteUserCommand(id), cancellationToken));
}
