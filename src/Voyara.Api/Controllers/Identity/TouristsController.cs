using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyara.Api.Authorization;
using Voyara.Application.DTOs.Identity;
using Voyara.Api.Security;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.DTOs;
using Voyara.Domain.Entities.Identity;
using Voyara.Infrastructure.Persistence;
using UserEntity = Voyara.Domain.Entities.Identity.User;

namespace Voyara.Api.Controllers.Identity;

/// <summary>
/// Tourist accounts (user + tourist profile). Anyone can register; a tourist can read, update and delete
/// only their own account, and an administrator can do so for every tourist.
/// </summary>
[Route("api/v1/tourists")]
public class TouristsController : AccountControllerBase
{
    public TouristsController(VoyaraDbContext context, ICurrentUser currentUser)
        : base(context, currentUser)
    {
    }

    /// <summary>Searches tourists by name or e-mail with paging. Admin only.</summary>
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<ActionResult<PagedResult<TouristReadDto>>> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TouristProfile> query = Context.TouristProfiles.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(p => p.User.FullName.ToLower().Contains(term) || p.User.Email.Contains(term));
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var total = await query.CountAsync(cancellationToken);
        var profiles = await query
            .Include(p => p.User)
            .Include(p => p.Interests)
            .OrderBy(p => p.User.FullName).ThenBy(p => p.UserId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Ok(new PagedResult<TouristReadDto>(profiles.Select(TouristReadDto.FromEntity).ToList(), total, page, pageSize));
    }

    /// <summary>Gets a tourist by user identifier. Admin or the tourist themselves.</summary>
    [HttpGet("{userId:guid}")]
    [Authorize]
    public async Task<ActionResult<TouristReadDto>> GetById(Guid userId, CancellationToken cancellationToken)
    {
        if (!IsAdminOrSelf(userId))
        {
            return Forbid();
        }

        var profile = await Context.TouristProfiles.AsNoTracking()
            .Include(p => p.User)
            .Include(p => p.Interests)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        if (profile is null)
        {
            return NotFound();
        }

        return Ok(TouristReadDto.FromEntity(profile));
    }

    /// <summary>Registers a tourist: creates the account (Tourist role) and the profile.</summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<TouristReadDto>> Register(RegisterTouristDto dto, CancellationToken cancellationToken)
    {
        if (await EmailTakenAsync(dto.Email, cancellationToken))
        {
            return Conflict($"An account with e-mail '{dto.Email.Trim()}' already exists.");
        }

        UserEntity? user = null;
        TouristProfile? profile = null;
        var failure = ApplyDomainChange(() =>
        {
            user = UserEntity.Create(dto.FullName, dto.Email, PasswordHashing.Hash(dto.Password));
            profile = TouristProfile.Create(user, dto.NationalityCode, dto.PreferredLanguage, dto.Budget, dto.PhotoUrl);
        });
        if (failure is not null)
        {
            return failure;
        }

        user!.AssignRole(await GetOrCreateRoleAsync(Roles.Tourist, cancellationToken));

        Context.Users.Add(user);
        Context.TouristProfiles.Add(profile!);
        await Context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { userId = user.Id }, TouristReadDto.FromEntity(profile!));
    }

    /// <summary>Updates a tourist's name and profile. Admin or the tourist themselves.</summary>
    [HttpPut("{userId:guid}")]
    [Authorize]
    public async Task<ActionResult<TouristReadDto>> Update(Guid userId, UpdateTouristDto dto, CancellationToken cancellationToken)
    {
        if (!IsAdminOrSelf(userId))
        {
            return Forbid();
        }

        var profile = await Context.TouristProfiles
            .Include(p => p.User)
            .Include(p => p.Interests)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        if (profile is null)
        {
            return NotFound();
        }

        var failure = ApplyDomainChange(() =>
        {
            profile.User.Rename(dto.FullName);
            profile.ChangeNationality(dto.NationalityCode);
            profile.ChangePreferredLanguage(dto.PreferredLanguage);
            profile.ChangeBudget(dto.Budget);
            profile.ChangePhoto(dto.PhotoUrl);
        });
        if (failure is not null)
        {
            return failure;
        }

        await Context.SaveChangesAsync(cancellationToken);

        return Ok(TouristReadDto.FromEntity(profile));
    }

    /// <summary>Soft-deletes the tourist's account. Admin or the tourist themselves.</summary>
    [HttpDelete("{userId:guid}")]
    [Authorize]
    public async Task<ActionResult> Delete(Guid userId, CancellationToken cancellationToken)
    {
        if (!IsAdminOrSelf(userId))
        {
            return Forbid();
        }

        var user = await FindUserAsync(userId, Roles.Tourist, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        SoftDelete(user);
        await Context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
