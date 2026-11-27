using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Voyara.Api.Authorization;
using Voyara.Api.Contracts.AiAssistant;
using Voyara.Application.AiAssistant.Commands.ArchiveConversation;
using Voyara.Application.AiAssistant.Commands.CompleteConversation;
using Voyara.Application.AiAssistant.Commands.StartConversation;
using Voyara.Application.AiAssistant.Queries.GetConversationById;
using Voyara.Application.AiAssistant.Queries.GetMyConversations;

namespace Voyara.Api.Controllers.AiAssistant;

/// <summary>Conversations between a tourist and the AI travel assistant.</summary>
[Authorize(Policy = AuthorizationPolicies.TouristOnly)]
[Route("api/v1/ai/conversations")]
public sealed class AiConversationsController : BaseApiController
{
    /// <summary>Starts a new conversation and returns its id. The body is optional.</summary>
    /// <param name="request">Optional conversation language.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost]
    public async Task<IActionResult> Start(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] StartConversationRequest? request,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new StartConversationCommand(request?.Language),
            cancellationToken));

    /// <summary>Lists the caller's conversations, newest first (archived ones are hidden by default).</summary>
    /// <param name="request">State filter and paging taken from the query string.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] ListConversationsRequest request,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new GetMyConversationsQuery(request.State, request.Page, request.PageSize),
            cancellationToken));

    /// <summary>Gets one of the caller's conversations with its requirements and message history.</summary>
    /// <param name="id">The conversation identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new GetConversationByIdQuery(id), cancellationToken));

    /// <summary>Marks a conversation as completed.</summary>
    /// <param name="id">The conversation identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new CompleteConversationCommand(id), cancellationToken));

    /// <summary>Archives a conversation (hides it from the default list).</summary>
    /// <param name="id">The conversation identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("{id:guid}/archive")]
    public async Task<IActionResult> Archive(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new ArchiveConversationCommand(id), cancellationToken));
}
