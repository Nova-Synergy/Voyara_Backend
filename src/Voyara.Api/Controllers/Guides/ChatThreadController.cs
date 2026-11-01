using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyara.Api.DTOs.Guides;
using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.Guides;
using Voyara.Infrastructure.Persistence;

namespace Voyara.Api.Controllers.Guides;

/// <summary>Chat between the tourist and the guide of an accepted request. Only those two can use a thread.</summary>
[ApiController]
[Route("api/v1/chat-threads")]
public class ChatThreadsController : ControllerBase
{
    private readonly VoyaraDbContext _context;
    private readonly ICurrentUser _currentUser;

    public ChatThreadsController(VoyaraDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    [HttpGet]
    [Route("{threadId:guid}/messages")]
    public async Task<ActionResult<List<ChatMessageReadDto>>> GetMessages(Guid threadId)
    {
        var thread = await _context.Set<ChatThread>()
            .AsNoTracking()
            .Include(t => t.GuideRequest)
                .ThenInclude(r => r.GuideProfile)
            .FirstOrDefaultAsync(t => t.Id == threadId);

        if (thread is null)
        {
            return NotFound();
        }

        if (!IsParticipant(thread))
        {
            return Forbid();
        }

        var messages = await _context.Set<ChatMessage>()
            .AsNoTracking()
            .Where(m => m.ChatThreadId == threadId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        return Ok(messages.Select(ChatMessageReadDto.FromEntity).ToList());
    }

    [HttpPost]
    [Route("{threadId:guid}/messages")]
    public async Task<ActionResult<ChatMessageReadDto>> SendMessage(Guid threadId, SendChatMessageDto dto)
    {
        if (_currentUser.UserId is not { } userId)
        {
            return Unauthorized();
        }

        var thread = await _context.Set<ChatThread>()
            .Include(t => t.GuideRequest)
                .ThenInclude(r => r.GuideProfile)
            .FirstOrDefaultAsync(t => t.Id == threadId);

        if (thread is null)
        {
            return NotFound();
        }

        if (!IsParticipant(thread))
        {
            return Forbid();
        }

        // The entity rejects messages on a closed thread.
        ChatMessage? message = null;
        var failure = ApplyDomainChange(() =>
            message = thread.PostMessage(
                userId,
                dto.Text,
                thread.GuideRequest.UserId,
                thread.GuideRequest.GuideProfile!.UserId,
                DateTimeOffset.UtcNow));
        if (failure is not null)
        {
            return failure;
        }

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMessages), new { threadId }, ChatMessageReadDto.FromEntity(message!));
    }

    private bool IsParticipant(ChatThread thread)
    {
        var userId = _currentUser.UserId;
        return thread.GuideRequest.UserId == userId || thread.GuideRequest.GuideProfile?.UserId == userId;
    }

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
