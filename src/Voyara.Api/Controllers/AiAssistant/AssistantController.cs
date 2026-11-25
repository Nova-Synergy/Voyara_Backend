using Microsoft.AspNetCore.Mvc;
using Voyara.Application.AiAssistant.Commands.ArchiveConversation;
using Voyara.Application.AiAssistant.Commands.CreateConversation;
using Voyara.Application.AiAssistant.Commands.SendMessage;
using Voyara.Application.AiAssistant.Queries.GetConversationById;
using Voyara.Application.AiAssistant.Queries.GetMyConversations;
using Voyara.Application.DTOs;

namespace Voyara.Api.Controllers.AiAssistant;

/// <summary>
/// AI Travel Assistant conversation management and chat endpoints powered by local Ollama LLM.
/// </summary>
[Route("api/v1/[controller]")]
public sealed class AssistantController : BaseApiController
{
    /// <summary>
    /// Starts a new conversational planning session with the AI assistant.
    /// </summary>
    /// <param name="request">Optional language preference payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("conversations")]
    public async Task<IActionResult> CreateConversation(
        [FromBody] CreateConversationRequest? request,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(
            new CreateConversationCommand(request?.Language ?? "en"),
            cancellationToken));

    /// <summary>
    /// Retrieves all AI planning conversations for the authenticated tourist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("conversations")]
    public async Task<IActionResult> GetMyConversations(CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(
            new GetMyConversationsQuery(),
            cancellationToken));

    /// <summary>
    /// Retrieves a specific conversation session and its complete message history.
    /// </summary>
    /// <param name="id">The conversation identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("conversations/{id:guid}")]
    public async Task<IActionResult> GetConversationById(
        Guid id,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(
            new GetConversationByIdQuery(id),
            cancellationToken));

    /// <summary>
    /// Sends a prompt/message to the AI assistant within an active session and receives the LLM completion.
    /// </summary>
    /// <param name="id">The conversation identifier.</param>
    /// <param name="request">The message content payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("conversations/{id:guid}/messages")]
    public async Task<IActionResult> SendMessage(
        Guid id,
        [FromBody] SendMessageRequest request,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(
            new SendMessageToAssistantCommand(id, request.Content),
            cancellationToken));

    /// <summary>
    /// Archives a conversation session.
    /// </summary>
    /// <param name="id">The conversation identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("conversations/{id:guid}/archive")]
    public async Task<IActionResult> ArchiveConversation(
        Guid id,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(
            new ArchiveConversationCommand(id),
            cancellationToken));
}
