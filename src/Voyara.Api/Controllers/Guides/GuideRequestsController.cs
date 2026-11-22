using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Contracts.Guides;
using Voyara.Application.Guides.Commands.AcceptGuideRequest;
using Voyara.Application.Guides.Commands.CancelGuideRequest;
using Voyara.Application.Guides.Commands.CompleteGuideRequest;
using Voyara.Application.Guides.Commands.CreateGuideRequest;
using Voyara.Application.Guides.Commands.RejectGuideRequest;
using Voyara.Application.Guides.Queries.GetGuideRequestById;
using Voyara.Application.Guides.Queries.GetIncomingGuideRequests;
using Voyara.Application.Guides.Queries.GetMyGuideRequests;
using Voyara.Domain.Enums;

namespace Voyara.Api.Controllers.Guides;

/// <summary>Guide requests: tourists create, list and cancel; guides list incoming requests, accept, reject and complete.</summary>
[ApiController]
[Authorize]
[Route("api/v1/guide-requests")]
public sealed class GuideRequestsController : BaseApiController
{
    /// <summary>Creates a request from the current tourist to a guide.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGuideRequestRequest request, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new CreateGuideRequestCommand(
                request.GuideProfileId, request.Date, request.StartTime, request.DurationHours,
                request.PartySize, request.PreferredLanguage, request.SpecialRequirements),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : HandleResult(result);
    }

    /// <summary>Lists the current tourist's requests, optionally filtered by status.</summary>
    [HttpGet]
    public async Task<IActionResult> GetMine(
        [FromQuery] RequestStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(new GetMyGuideRequestsQuery(status, page, pageSize), cancellationToken));

    /// <summary>Lists the requests addressed to the current guide, optionally filtered by status.</summary>
    [HttpGet("incoming")]
    public async Task<IActionResult> GetIncoming(
        [FromQuery] RequestStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(new GetIncomingGuideRequestsQuery(status, page, pageSize), cancellationToken));

    /// <summary>Gets one request (tourist owner or addressed guide only).</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new GetGuideRequestByIdQuery(id), cancellationToken));

    /// <summary>Accepts a pending request; creates its session and chat thread.</summary>
    [HttpPut("{id:guid}/accept")]
    public async Task<IActionResult> Accept(
        Guid id, [FromBody] AcceptGuideRequestRequest request, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new AcceptGuideRequestCommand(id, request.MeetingLocation, request.AgreedPrice, request.Currency),
            cancellationToken));

    /// <summary>Rejects a pending request with a reason.</summary>
    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid id, [FromBody] RejectGuideRequestRequest request, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new RejectGuideRequestCommand(id, request.Reason), cancellationToken));

    /// <summary>Cancels the current tourist's request.</summary>
    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new CancelGuideRequestCommand(id), cancellationToken));

    /// <summary>Marks an accepted request as completed (guide only), which allows the tourist to review it.</summary>
    [HttpPut("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new CompleteGuideRequestCommand(id), cancellationToken));
}
