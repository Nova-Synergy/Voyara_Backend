using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyara.Application.DTOs.Identity;
using Voyara.Api.Security;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.DTOs;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Enums;
using Voyara.Infrastructure.Persistence;
using UserEntity = Voyara.Domain.Entities.Identity.User;

namespace Voyara.Api.Controllers.Identity;

/// <summary>
/// Tour guide accounts (user + guide profile). Anyone can apply (the profile starts as Pending); the public sees
/// approved guides only; a guide, or an administrator, manages the profile.
/// </summary>
[Route("api/v1/tour-guides")]
public class TourGuidesController : AccountControllerBase
{
    public TourGuidesController(VoyaraDbContext context, ICurrentUser currentUser)
        : base(context, currentUser)
    {
    }

    /// <summary>
    /// Lists guides with paging. The public sees approved guides; an administrator can filter by status.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<TourGuideReadDto>>> Search(
        [FromQuery] int? cityId,
        [FromQuery] GuideStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        IQueryable<GuideProfile> query = Context.Set<GuideProfile>().AsNoTracking().Where(g => !g.IsDeleted);

        // Only an administrator may look beyond approved guides.
        var effectiveStatus = IsAdmin ? status : GuideStatus.Approved;
        if (effectiveStatus is { } s)
        {
            query = query.Where(g => g.Status == s);
        }

        if (cityId is { } city)
        {
            query = query.Where(g => g.CityId == city);
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var total = await query.CountAsync(cancellationToken);
        var guides = await query
            .Include(g => g.User)
            .OrderByDescending(g => g.AverageRating).ThenBy(g => g.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Ok(new PagedResult<TourGuideReadDto>(guides.Select(TourGuideReadDto.FromEntity).ToList(), total, page, pageSize));
    }

    /// <summary>Gets a guide profile. Non-approved profiles are visible only to the owner and administrators.</summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<TourGuideReadDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var profile = await Context.Set<GuideProfile>().AsNoTracking()
            .Include(g => g.User)
            .FirstOrDefaultAsync(g => g.Id == id && !g.IsDeleted, cancellationToken);

        if (profile is null || (profile.Status != GuideStatus.Approved && !IsAdminOrSelf(profile.UserId)))
        {
            return NotFound();
        }

        return Ok(TourGuideReadDto.FromEntity(profile));
    }

    /// <summary>Registers a tour guide: creates the account (TourGuide role) and a Pending profile.</summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<TourGuideReadDto>> Register(RegisterTourGuideDto dto, CancellationToken cancellationToken)
    {
        if (!await Context.Cities.AsNoTracking().AnyAsync(c => c.Id == dto.CityId && c.IsActive, cancellationToken))
        {
            return BadRequest($"City '{dto.CityId}' was not found.");
        }

        if (await EmailTakenAsync(dto.Email, cancellationToken))
        {
            return Conflict($"An account with e-mail '{dto.Email.Trim()}' already exists.");
        }

        UserEntity? user = null;
        GuideProfile? profile = null;
        var failure = ApplyDomainChange(() =>
        {
            user = UserEntity.Create(dto.FullName, dto.Email, PasswordHashing.Hash(dto.Password));
            profile = GuideProfile.Create(user.Id, dto.CityId, dto.Bio, dto.PhotoUrl, dto.YearsOfExperience);
        });
        if (failure is not null)
        {
            return failure;
        }

        user!.AssignRole(await GetOrCreateRoleAsync(Roles.TourGuide, cancellationToken));

        Context.Users.Add(user);
        Context.Set<GuideProfile>().Add(profile!);
        await Context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = profile!.Id }, TourGuideReadDto.FromEntity(profile));
    }

    /// <summary>Updates a guide's name and profile details. The owner or an administrator.</summary>
    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<TourGuideReadDto>> Update(Guid id, UpdateTourGuideDto dto, CancellationToken cancellationToken)
    {
        var profile = await Context.Set<GuideProfile>()
            .Include(g => g.User)
            .FirstOrDefaultAsync(g => g.Id == id && !g.IsDeleted, cancellationToken);
        if (profile is null)
        {
            return NotFound();
        }

        if (!IsAdminOrSelf(profile.UserId))
        {
            return Forbid();
        }

        var failure = ApplyDomainChange(() =>
        {
            profile.UpdateDetails(dto.Bio, dto.PhotoUrl, dto.YearsOfExperience);
            profile.User!.Rename(dto.FullName);
        });
        if (failure is not null)
        {
            return failure;
        }

        await Context.SaveChangesAsync(cancellationToken);

        return Ok(TourGuideReadDto.FromEntity(profile));
    }

    /// <summary>Soft-deletes the guide profile (the user account stays). The owner or an administrator.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var profile = await Context.Set<GuideProfile>()
            .FirstOrDefaultAsync(g => g.Id == id && !g.IsDeleted, cancellationToken);
        if (profile is null)
        {
            return NotFound();
        }

        if (!IsAdminOrSelf(profile.UserId))
        {
            return Forbid();
        }

        profile.IsDeleted = true;
        await Context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
