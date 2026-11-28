using Microsoft.AspNetCore.Mvc;
using Voyara.Application.AiAssistant.Commands.ArchiveConversation;
using Voyara.Application.AiAssistant.Commands.CreateConversation;
using Voyara.Application.AiAssistant.Commands.RequestTravelRecommendations;
using Voyara.Application.AiAssistant.Commands.SendMessage;
using Voyara.Application.AiAssistant.Commands.UpdateRequirements;
using Voyara.Application.AiAssistant.Queries.GetConversationById;
using Voyara.Application.AiAssistant.Queries.GetConversationContext;
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
    /// Retrieves AI planning conversations for the authenticated tourist with paging.
    /// </summary>
    /// <param name="page">1-based page number.</param>
    /// <param name="pageSize">Number of conversations per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("conversations")]
    public async Task<IActionResult> GetMyConversations(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(
            new GetMyConversationsQuery(page, pageSize),
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
    /// Retrieves the assembled AI planning context and matching catalog destinations for a conversation.
    /// </summary>
    /// <param name="id">The conversation identifier.</param>
    /// <param name="searchHint">Optional keyword to filter catalog destinations.</param>
    /// <param name="maxDestinations">Maximum number of catalog destinations to include (default: 5).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("conversations/{id:guid}/context")]
    public async Task<IActionResult> GetConversationContext(
        Guid id,
        [FromQuery] string? searchHint = null,
        [FromQuery] int maxDestinations = 5,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(
            new GetConversationContextQuery(id, searchHint, maxDestinations),
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

    /// <summary>
    /// Requests interactive conversational travel recommendations from the assistant.
    /// </summary>
    /// <param name="id">The conversation identifier.</param>
    /// <param name="request">Optional recommendation filters and inquiry prompt.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("conversations/{id:guid}/recommendations")]
    public async Task<IActionResult> RequestRecommendations(
        Guid id,
        [FromBody] RequestRecommendationPromptRequest? request,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(
            new RequestTravelRecommendationsCommand(
                id,
                request?.Prompt,
                request?.CityId,
                request?.CategoryId,
                request?.Limit ?? 3),
            cancellationToken));

    /// <summary>
    /// Updates or applies extracted travel requirements to an ongoing planning conversation.
    /// </summary>
    /// <param name="id">The conversation identifier.</param>
    /// <param name="request">Structured trip requirements payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPut("conversations/{id:guid}/requirements")]
    public async Task<IActionResult> UpdateRequirements(
        Guid id,
        [FromBody] UpdateRequirementsRequest request,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(
            new UpdateConversationRequirementsCommand(
                id,
                request.Country,
                request.DurationDays,
                request.Interests,
                request.BudgetAmount,
                request.BudgetCurrency,
                request.TravelStyle,
                request.TransportPreference),
            cancellationToken));
}
