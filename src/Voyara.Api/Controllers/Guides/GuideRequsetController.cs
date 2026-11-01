using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyara.Api.Authorization;
using Voyara.Api.DTOs.Guides;
using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Enums;
using Voyara.Infrastructure.Persistence;

namespace Voyara.Api.Controllers.Guides;

/// <summary>Guide requests: a tourist creates and reads requests, and reviews the guide afterwards.</summary>
[ApiController]
[Route("api/v1/guide-requests")]
public class GuideRequestsController : ControllerBase
{
    private static readonly TimeSpan ResponseWindow = TimeSpan.FromHours(24);

    private readonly VoyaraDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GuideRequestsController(VoyaraDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }


    [HttpGet]
    public async Task<ActionResult<List<GuideRequestReadDto>>> GetMine([FromQuery] RequestStatus? status)
    {
        if (_currentUser.UserId is not { } userId)
        {
            return Unauthorized();
        }

        var query = _context.Set<GuideRequest>()
            .AsNoTracking()
            .Include(r => r.GuideSession)
            .Include(r => r.ChatThreads)
            .Include(r => r.GuideReview)
            .AsSplitQuery()
            .Where(r => r.UserId == userId);

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


    [HttpGet]
    [Route("GetGuideRequestById/{id:guid}")]
    public async Task<ActionResult<GuideRequestReadDto>> GetById(Guid id)
    {
        var request = await _context.Set<GuideRequest>()
            .AsNoTracking()
            .Include(r => r.GuideProfile)
            .Include(r => r.GuideSession)
            .Include(r => r.ChatThreads)
            .Include(r => r.GuideReview)
            .AsSplitQuery()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request is null)
        {
            return NotFound();
        }

        var userId = _currentUser.UserId;
        if (request.UserId != userId && request.GuideProfile?.UserId != userId)
        {
            return Forbid();
        }

        return Ok(GuideRequestReadDto.FromEntity(request));
    }

    [HttpPost]
    [Route("CreateGuideRequest")]
    public async Task<ActionResult<GuideRequestReadDto>> Create(CreateGuideRequestDto dto)
    {
        if (_currentUser.UserId is not { } userId)
        {
            return Unauthorized();
        }

        // The guide profile must exist (foreign key).
        var guideExists = await _context.Set<GuideProfile>().AnyAsync(p => p.Id == dto.GuideProfileId);
        if (!guideExists)
        {
            return BadRequest("The guide profile does not exist.");
        }

        GuideRequest? request = null;
        var failure = ApplyDomainChange(() =>
            request = GuideRequest.Create(
                userId,
                dto.GuideProfileId,
                dto.Date,
                dto.StartTime,
                dto.DurationHours,
                dto.PartySize,
                dto.PreferredLanguage,
                dto.SpecialRequirements,
                DateTimeOffset.UtcNow,
                ResponseWindow));
        if (failure is not null)
        {
            return failure;
        }

        _context.Set<GuideRequest>().Add(request!);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = request!.Id }, GuideRequestReadDto.FromEntity(request));
    }


    [HttpPost]
    [Route("{id:guid}/review")]
    public async Task<ActionResult<GuideReviewReadDto>> CreateReview(Guid id, CreateGuideReviewDto dto)
    {
        var request = await _context.Set<GuideRequest>()
            .Include(r => r.GuideProfile)
            .Include(r => r.GuideReview)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request is null)
        {
            return NotFound();
        }

        if (request.UserId != _currentUser.UserId)
        {
            return Forbid();
        }

        // The entity checks that the request is completed, not yet reviewed, and that the guide is approved.
        GuideReview? review = null;
        var failure = ApplyDomainChange(() =>
        {
            review = request.AddReview(dto.Rating, dto.Text);
            request.GuideProfile!.ApplyReview(dto.Rating);
        });
        if (failure is not null)
        {
            return failure;
        }

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id }, GuideReviewReadDto.FromEntity(review!));
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
