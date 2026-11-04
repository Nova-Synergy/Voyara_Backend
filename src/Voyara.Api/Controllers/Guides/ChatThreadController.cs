
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.DTOs.Guides;
using Voyara.Application.Guides;

namespace Voyara.Api.Controllers.Guides;

/// <summary>
/// Provides API endpoints for managing chat threads and their messages,
/// including retrieving messages and sending new messages.
/// </summary>
[ApiController]
[Route("api/v1/chat-threads")]
public sealed class ChatThreadsController : GuidesControllerBase
{
    /// <summary>
    /// Retrieves the messages of a specific chat thread using pagination.
    /// </summary>
    /// <param name="threadId">The unique identifier of the chat thread.</param>
    /// <param name="page">The page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of messages per page. Defaults to 10.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// An HTTP 200 response containing the paginated chat messages
    /// when the operation succeeds.
    /// </returns>
    [HttpGet]
    [Route("{threadId:guid}/messages")]
    public async Task<IActionResult> GetMessages(Guid threadId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        => Map(await Sender.Send(new GetChatMessagesQuery(threadId, page, pageSize), ct), Ok);

    /// <summary>
    /// Sends a new message to a specific chat thread.
    /// </summary>
    /// <param name="threadId">The unique identifier of the chat thread.</param>
    /// <param name="dto">The data required to send the chat message.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// An HTTP 201 Created response containing the sent message
    /// when the operation succeeds.
    /// </returns>
    [HttpPost]
    [Route("{threadId:guid}/messages")]
    public async Task<IActionResult> SendMessage(Guid threadId, SendChatMessageDto dto, CancellationToken ct)
        => Map(await Sender.Send(new SendChatMessageCommand(threadId, dto), ct),
            value => CreatedAtAction(nameof(GetMessages), new { threadId }, value));
}
