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

/// <summary>Admin CRUD over every user account, whatever its role.</summary>
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/v1/admin/users")]
public class UsersController : AccountControllerBase
{
    public UsersController(VoyaraDbContext context, ICurrentUser currentUser)
        : base(context, currentUser)
    {
    }

    /// <summary>Searches users by name or e-mail, optionally by role, with paging.</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<UserReadDto>>> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] string? role,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        IQueryable<UserEntity> query = Context.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(role))
        {
            if (!TryNormalizeRoles([role], out var roles, out _))
            {
                return BadRequest($"'{role}' is not a valid role. Allowed roles: Tourist, TourGuide, Admin.");
            }

            var roleName = roles[0];
            query = query.Where(u => u.Roles.Any(r => r.Name == roleName));
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(u => u.FullName.ToLower().Contains(term) || u.Email.Contains(term));
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var total = await query.CountAsync(cancellationToken);
        var users = await query
            .Include(u => u.Roles)
            .OrderBy(u => u.FullName).ThenBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Ok(new PagedResult<UserReadDto>(users.Select(UserReadDto.FromEntity).ToList(), total, page, pageSize));
    }

    /// <summary>Gets a user by identifier.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserReadDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await Context.Users.AsNoTracking().Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(UserReadDto.FromEntity(user));
    }

    /// <summary>Creates a user; the role defaults to Tourist.</summary>
    [HttpPost]
    public async Task<ActionResult<UserReadDto>> Create(CreateUserDto dto, CancellationToken cancellationToken)
    {
        var roles = new List<string> { Roles.Tourist };
        if (dto.Roles is { Count: > 0 } && !TryNormalizeRoles(dto.Roles, out roles, out var invalid))
        {
            return BadRequest($"'{invalid}' is not a valid role. Allowed roles: Tourist, TourGuide, Admin.");
        }

        if (await EmailTakenAsync(dto.Email, cancellationToken))
        {
            return Conflict($"An account with e-mail '{dto.Email.Trim()}' already exists.");
        }

        UserEntity? user = null;
        var failure = ApplyDomainChange(() => user = UserEntity.Create(dto.FullName, dto.Email, PasswordHashing.Hash(dto.Password)));
        if (failure is not null)
        {
            return failure;
        }

        foreach (var roleName in roles)
        {
            user!.AssignRole(await GetOrCreateRoleAsync(roleName, cancellationToken));
        }

        Context.Users.Add(user!);
        await Context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = user!.Id }, UserReadDto.FromEntity(user));
    }

    /// <summary>Updates a user's name, roles and active state.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserReadDto>> Update(Guid id, UpdateUserDto dto, CancellationToken cancellationToken)
    {
        var user = await FindUserAsync(id, null, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        List<string>? targetRoles = null;
        if (dto.Roles is not null)
        {
            if (!TryNormalizeRoles(dto.Roles, out var normalized, out var invalid))
            {
                return BadRequest(invalid is null
                    ? "A user must keep at least one role."
                    : $"'{invalid}' is not a valid role. Allowed roles: Tourist, TourGuide, Admin.");
            }

            // An administrator cannot lock themselves out of the admin area.
            if (user.Id == CurrentUser.UserId && !normalized.Contains(Roles.Admin))
            {
                return Conflict("You cannot remove the Admin role from your own account.");
            }

            targetRoles = normalized;
        }

        var failure = ApplyDomainChange(() => user.Rename(dto.FullName)) ?? ApplyActivation(user, dto.IsActive);
        if (failure is not null)
        {
            return failure;
        }

        if (targetRoles is not null)
        {
            await ReplaceRolesAsync(user, targetRoles, cancellationToken);
        }

        await Context.SaveChangesAsync(cancellationToken);

        return Ok(UserReadDto.FromEntity(user));
    }

    /// <summary>Soft-deletes a user and revokes their sessions.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        if (id == CurrentUser.UserId)
        {
            return Conflict("You cannot delete your own account.");
        }

        var user = await FindUserAsync(id, null, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        SoftDelete(user);
        await Context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
