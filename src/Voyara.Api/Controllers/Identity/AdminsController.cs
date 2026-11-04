using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Authorization;
using Voyara.Application.DTOs.Identity;
using Voyara.Application.Identity.Commands.Admins.CreateAdmin;
using Voyara.Application.Identity.Commands.Admins.DeleteAdmin;
using Voyara.Application.Identity.Commands.Admins.UpdateAdmin;
using Voyara.Application.Identity.Queries.Admins.GetAdminById;
using Voyara.Application.Identity.Queries.Admins.SearchAdmins;

namespace Voyara.Api.Controllers.Identity;

/// <summary>Admin CRUD over administrator accounts.</summary>
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/v1/admin/admins")]
public sealed class AdminsController : IdentityControllerBase
{
    /// <summary>Searches administrators by name or e-mail with paging.</summary>
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(
            new SearchAdminsQuery(searchTerm, page, pageSize), cancellationToken));

    /// <summary>Gets an administrator by identifier.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new GetAdminByIdQuery(id), cancellationToken));

    /// <summary>Creates an administrator account.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(CreateAdminDto dto, CancellationToken cancellationToken)
        => HandleCreated(
            await Sender.Send(
                new CreateAdminCommand(dto.FullName, dto.Email, dto.Password),
                cancellationToken),
            nameof(GetById),
            u => new { id = u.Id });

    /// <summary>Updates an administrator's name and active state.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateAdminDto dto, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new UpdateAdminCommand(id, dto.FullName, dto.IsActive), cancellationToken));

    /// <summary>Soft-deletes an administrator account.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => HandleNoContent(await Sender.Send(new DeleteAdminCommand(id), cancellationToken));
}
