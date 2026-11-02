using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyara.Api.Authorization;
using Voyara.Application.DTOs.Identity;
using Voyara.Api.Security;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.DTOs;
using Voyara.Infrastructure.Persistence;
using UserEntity = Voyara.Domain.Entities.Identity.User;

namespace Voyara.Api.Controllers.Identity;

/// <summary>Admin CRUD over administrator accounts (users holding the Admin role).</summary>
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/v1/admin/admins")]
public class AdminsController : AccountControllerBase
{
    public AdminsController(VoyaraDbContext context, ICurrentUser currentUser)
        : base(context, currentUser)
    {
    }

    /// <summary>Searches administrators by name or e-mail with paging.</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<UserReadDto>>> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        IQueryable<UserEntity> query = Context.Users.AsNoTracking()
            .Where(u => u.Roles.Any(r => r.Name == Roles.Admin));

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(u => u.FullName.ToLower().Contains(term) || u.Email.Contains(term));
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var total = await query.CountAsync(cancellationToken);
        var admins = await query
            .Include(u => u.Roles)
            .OrderBy(u => u.FullName).ThenBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Ok(new PagedResult<UserReadDto>(admins.Select(UserReadDto.FromEntity).ToList(), total, page, pageSize));
    }

    /// <summary>Gets an administrator by identifier.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserReadDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var admin = await Context.Users.AsNoTracking().Include(u => u.Roles)
            .Where(u => u.Roles.Any(r => r.Name == Roles.Admin))
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (admin is null)
        {
            return NotFound();
        }

        return Ok(UserReadDto.FromEntity(admin));
    }

    /// <summary>Creates an administrator account.</summary>
    [HttpPost]
    public async Task<ActionResult<UserReadDto>> Create(CreateAdminDto dto, CancellationToken cancellationToken)
    {
        if (await EmailTakenAsync(dto.Email, cancellationToken))
        {
            return Conflict($"An account with e-mail '{dto.Email.Trim()}' already exists.");
        }

        UserEntity? admin = null;
        var failure = ApplyDomainChange(() => admin = UserEntity.Create(dto.FullName, dto.Email, PasswordHashing.Hash(dto.Password)));
        if (failure is not null)
        {
            return failure;
        }

        admin!.AssignRole(await GetOrCreateRoleAsync(Roles.Admin, cancellationToken));

        Context.Users.Add(admin);
        await Context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = admin.Id }, UserReadDto.FromEntity(admin));
    }

    /// <summary>Updates an administrator's name and active state.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserReadDto>> Update(Guid id, UpdateAdminDto dto, CancellationToken cancellationToken)
    {
        var admin = await FindUserAsync(id, Roles.Admin, cancellationToken);
        if (admin is null)
        {
            return NotFound();
        }

        var failure = ApplyDomainChange(() => admin.Rename(dto.FullName)) ?? ApplyActivation(admin, dto.IsActive);
        if (failure is not null)
        {
            return failure;
        }

        await Context.SaveChangesAsync(cancellationToken);

        return Ok(UserReadDto.FromEntity(admin));
    }

    /// <summary>Soft-deletes an administrator account.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        if (id == CurrentUser.UserId)
        {
            return Conflict("You cannot delete your own account.");
        }

        var admin = await FindUserAsync(id, Roles.Admin, cancellationToken);
        if (admin is null)
        {
            return NotFound();
        }

        SoftDelete(admin);
        await Context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
