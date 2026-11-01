using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyara.Api.Authorization;
using Voyara.Api.DTOs.Guides;
using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Entities.LocalInfo;
using Voyara.Domain.Enums;
using Voyara.Infrastructure.Persistence;

namespace Voyara.Api.Controllers.Guides;

/// <summary>
/// Simple CRUD for the GuideProfile aggregate. Its child entities (languages, expertise, availability,
/// time off, pricing) are managed through nested routes, because they only exist inside a profile.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class GuidesController : ControllerBase
{
    private readonly VoyaraDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GuidesController(VoyaraDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    // ------------------------------------------------------------------ Guide profile

    [HttpGet]
    [Route("GetAllGuideProfiles")]
    public async Task<ActionResult<List<GuideProfileReadDto>>> GetAll()
    {
        var profiles = await _context.Set<GuideProfile>()
            .AsNoTracking()
            .ToListAsync();

        return Ok(profiles.Select(GuideProfileReadDto.FromEntity).ToList());
    }

    [HttpGet]
    [Route("GetGuideProfileById/{id:guid}")]
    public async Task<ActionResult<GuideProfileDetailDto>> GetById(Guid id)
    {
        var profile = await _context.Set<GuideProfile>()
            .AsNoTracking()
            .Include(p => p.Languages)
            .Include(p => p.Expertise)
            .Include(p => p.Availability)
            .Include(p => p.TimeOff)
            .Include(p => p.Pricing)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (profile is null)
        {
            return NotFound();
        }

        return Ok(GuideProfileDetailDto.FromEntity(profile));
    }

    [HttpPost]
    public async Task<ActionResult<GuideProfileReadDto>> Create(CreateGuideProfileDto createGuideProfileDto)
    {
        if (_currentUser.UserId is not { } userId)
        {
            return Unauthorized();
        }

        // The city must exist (foreign key).
        var cityExists = await _context.Set<City>().AnyAsync(c => c.Id == createGuideProfileDto.CityId);
        if (!cityExists)
        {
            return BadRequest("The city does not exist.");
        }

        // One guide profile per user (unique index on UserId). Soft-deleted rows still hold the index.
        var alreadyHasProfile = await _context.Set<GuideProfile>()
            .IgnoreQueryFilters()
            .AnyAsync(p => p.UserId == userId);
        if (alreadyHasProfile)
        {
            return Conflict("This user already has a guide profile.");
        }

        GuideProfile? profile = null;
        var failure = ApplyDomainChange(() =>
            profile = GuideProfile.Create(userId, createGuideProfileDto.CityId, createGuideProfileDto.Bio, createGuideProfileDto.PhotoUrl, createGuideProfileDto.YearsOfExperience));
        if (failure is not null)
        {
            return failure;
        }

        _context.Set<GuideProfile>().Add(profile!);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = profile!.Id }, GuideProfileReadDto.FromEntity(profile));
    }

    [HttpPut]
    [Route("UpdateGuideProfile/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateGuideProfileDto dto)
    {
        var profile = await _context.Set<GuideProfile>().FirstOrDefaultAsync(p => p.Id == id);
        if (profile is null)
        {
            return NotFound();
        }

        if (profile.UserId != _currentUser.UserId)
        {
            return Forbid();
        }

        // Properties have private setters, so the entity's own method applies the change.
        var failure = ApplyDomainChange(() =>
            profile.UpdateDetails(dto.Bio, dto.PhotoUrl, dto.YearsOfExperience));
        if (failure is not null)
        {
            return failure;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var profile = await _context.Set<GuideProfile>().FirstOrDefaultAsync(p => p.Id == id);
        if (profile is null)
        {
            return NotFound();
        }

        if (profile.UserId != _currentUser.UserId)
        {
            return Forbid();
        }

        _context.Set<GuideProfile>().Remove(profile);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ------------------------------------------------------------------ Reviews and requests of a guide

    [HttpGet]
    [Route("{id:guid}/reviews")]
    public async Task<ActionResult<List<GuideReviewReadDto>>> GetReviews(Guid id)
    {
        var profileExists = await _context.Set<GuideProfile>().AnyAsync(p => p.Id == id);
        if (!profileExists)
        {
            return NotFound();
        }

        var reviews = await _context.Set<GuideReview>()
            .AsNoTracking()
            .Where(r => r.GuideProfileId == id)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Ok(reviews.Select(GuideReviewReadDto.FromEntity).ToList());
    }

    [HttpGet]
    [Route("{id:guid}/requests")]
    public async Task<ActionResult<List<GuideRequestReadDto>>> GetRequests(Guid id, [FromQuery] RequestStatus? status)
    {
        var profile = await _context.Set<GuideProfile>().AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (profile is null)
        {
            return NotFound();
        }

        if (profile.UserId != _currentUser.UserId)
        {
            return Forbid();
        }

        var query = _context.Set<GuideRequest>()
            .AsNoTracking()
            .Include(r => r.GuideSession)
            .Include(r => r.ChatThreads)
            .Include(r => r.GuideReview)
            .AsSplitQuery()
            .Where(r => r.GuideProfileId == id);

        if (status is { } requestStatus)
        {
            query = query.Where(r => r.Status == requestStatus);
        }

        var requests = await query
            .OrderByDescending(r => r.Date)
            .ThenByDescending(r => r.StartTime)
            .ToListAsync();

        return Ok(requests.Select(GuideRequestReadDto.FromEntity).ToList());
    }

    // ------------------------------------------------------------------ Child collections

    /// <summary>Adds a language to the guide profile (owner only).</summary>
    [HttpPost]
    [Route("{id:guid}/languages")]
    public async Task<ActionResult<GuideLanguageReadDto>> AddLanguage(Guid id, CreateGuideLanguageDto dto)
    {
        var (profile, error) = await GetOwnProfileAsync(id);
        if (profile is null)
        {
            return error!;
        }

        var oldIds = profile.Languages.Select(item => item.Id).ToHashSet();

        var failure = ApplyDomainChange(() => profile.AddLanguage(dto.LanguageCode, dto.Proficiency));
        if (failure is not null)
        {
            return failure;
        }

        await _context.SaveChangesAsync();

        var added = profile.Languages.First(item => !oldIds.Contains(item.Id));
        return CreatedAtAction(nameof(GetById), new { id }, GuideLanguageReadDto.FromEntity(added));
    }

    [HttpDelete]
    [Route("{id:guid}/languages/{itemId:guid}")]
    public async Task<IActionResult> RemoveLanguage(Guid id, Guid itemId)
    {
        var (profile, error) = await GetOwnProfileAsync(id);
        if (profile is null)
        {
            return error!;
        }

        var item = profile.Languages.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
        {
            return NotFound();
        }

        var failure = ApplyDomainChange(() => profile.RemoveLanguage(item.LanguageCode));
        if (failure is not null)
        {
            return failure;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost]
    [Route("{id:guid}/expertise")]
    public async Task<ActionResult<GuideExpertiseReadDto>> AddExpertise(Guid id, CreateGuideExpertiseDto dto)
    {
        var (profile, error) = await GetOwnProfileAsync(id);
        if (profile is null)
        {
            return error!;
        }

        var oldIds = profile.Expertise.Select(item => item.Id).ToHashSet();

        var failure = ApplyDomainChange(() => profile.AddExpertise(dto.CategoryId));
        if (failure is not null)
        {
            return failure;
        }

        await _context.SaveChangesAsync();

        var added = profile.Expertise.First(item => !oldIds.Contains(item.Id));
        return CreatedAtAction(nameof(GetById), new { id }, GuideExpertiseReadDto.FromEntity(added));
    }


    [HttpDelete]
    [Route("{id:guid}/expertise/{itemId:guid}")]
    public async Task<IActionResult> RemoveExpertise(Guid id, Guid itemId)
    {
        var (profile, error) = await GetOwnProfileAsync(id);
        if (profile is null)
        {
            return error!;
        }

        var item = profile.Expertise.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
        {
            return NotFound();
        }

        var failure = ApplyDomainChange(() => profile.RemoveExpertise(item.CategoryId));
        if (failure is not null)
        {
            return failure;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpPost]
    [Route("{id:guid}/availability")]
    public async Task<ActionResult<GuideAvailabilityReadDto>> AddAvailability(Guid id, CreateGuideAvailabilityDto dto)
    {
        var (profile, error) = await GetOwnProfileAsync(id);
        if (profile is null)
        {
            return error!;
        }

        var oldIds = profile.Availability.Select(item => item.Id).ToHashSet();

        var failure = ApplyDomainChange(() => profile.AddAvailability(dto.Day, dto.StartTime, dto.EndTime));
        if (failure is not null)
        {
            return failure;
        }

        await _context.SaveChangesAsync();

        var added = profile.Availability.First(item => !oldIds.Contains(item.Id));
        return CreatedAtAction(nameof(GetById), new { id }, GuideAvailabilityReadDto.FromEntity(added));
    }


    [HttpDelete]
    [Route("{id:guid}/availability/{itemId:guid}")]
    public async Task<IActionResult> RemoveAvailability(Guid id, Guid itemId)
    {
        var (profile, error) = await GetOwnProfileAsync(id);
        if (profile is null)
        {
            return error!;
        }

        var item = profile.Availability.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
        {
            return NotFound();
        }

        var failure = ApplyDomainChange(() => profile.RemoveAvailability(item.Day, item.StartTime));
        if (failure is not null)
        {
            return failure;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpPost]
    [Route("{id:guid}/time-off")]
    public async Task<ActionResult<GuideTimeOffReadDto>> AddTimeOff(Guid id, CreateGuideTimeOffDto dto)
    {
        var (profile, error) = await GetOwnProfileAsync(id);
        if (profile is null)
        {
            return error!;
        }

        var oldIds = profile.TimeOff.Select(item => item.Id).ToHashSet();

        var failure = ApplyDomainChange(() => profile.AddTimeOff(dto.Date, dto.Reason));
        if (failure is not null)
        {
            return failure;
        }

        await _context.SaveChangesAsync();

        var added = profile.TimeOff.First(item => !oldIds.Contains(item.Id));
        return CreatedAtAction(nameof(GetById), new { id }, GuideTimeOffReadDto.FromEntity(added));
    }


    [HttpDelete]
    [Route("{id:guid}/time-off/{itemId:guid}")]
    public async Task<IActionResult> RemoveTimeOff(Guid id, Guid itemId)
    {
        var (profile, error) = await GetOwnProfileAsync(id);
        if (profile is null)
        {
            return error!;
        }

        var item = profile.TimeOff.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
        {
            return NotFound();
        }

        var failure = ApplyDomainChange(() => profile.RemoveTimeOff(item.Date));
        if (failure is not null)
        {
            return failure;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpPost]
    [Route("{id:guid}/pricing")]
    public async Task<ActionResult<GuidePricingReadDto>> AddPricing(Guid id, CreateGuidePricingDto dto)
    {
        var (profile, error) = await GetOwnProfileAsync(id);
        if (profile is null)
        {
            return error!;
        }

        var oldIds = profile.Pricing.Select(item => item.Id).ToHashSet();

        var failure = ApplyDomainChange(() => profile.AddPricing(dto.Unit, dto.Amount, dto.Currency));
        if (failure is not null)
        {
            return failure;
        }

        await _context.SaveChangesAsync();

        var added = profile.Pricing.First(item => !oldIds.Contains(item.Id));
        return CreatedAtAction(nameof(GetById), new { id }, GuidePricingReadDto.FromEntity(added));
    }


    [HttpDelete]
    [Route("{id:guid}/pricing/{itemId:guid}")]
    public async Task<IActionResult> RemovePricing(Guid id, Guid itemId)
    {
        var (profile, error) = await GetOwnProfileAsync(id);
        if (profile is null)
        {
            return error!;
        }

        var item = profile.Pricing.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
        {
            return NotFound();
        }

        var failure = ApplyDomainChange(() => profile.RemovePricing(item.Unit, item.Currency));
        if (failure is not null)
        {
            return failure;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ------------------------------------------------------------------ Helpers

    /// <summary>Loads a profile with its children for editing. Returns an error result when it is missing or not owned.</summary>
    private async Task<(GuideProfile? Profile, ActionResult? Error)> GetOwnProfileAsync(Guid id)
    {
        var profile = await _context.Set<GuideProfile>()
            .Include(p => p.Languages)
            .Include(p => p.Expertise)
            .Include(p => p.Availability)
            .Include(p => p.TimeOff)
            .Include(p => p.Pricing)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (profile is null)
        {
            return (null, NotFound());
        }

        if (profile.UserId != _currentUser.UserId)
        {
            return (null, Forbid());
        }

        return (profile, null);
    }

    /// <summary>Runs an entity method and turns its rule violations into 400 / 409 responses.</summary>
    private ActionResult? ApplyDomainChange(Action change)
    {
        try
        {
            change();
            return null;
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
