using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Authorization;
using Voyara.Api.Contracts.Guides;
using Voyara.Application.Guides.Commands.ApproveGuideProfile;
using Voyara.Application.Guides.Commands.RejectGuideProfile;
using Voyara.Application.Guides.Commands.SuspendGuideProfile;
using Voyara.Application.Guides.Queries.GetGuideApplications;
using Voyara.Application.Guides.Queries.GetGuideProfileDetails;
using Voyara.Domain.Enums;

namespace Voyara.Api.Controllers.Guides;

/// <summary>Admin review of guide applications (UC-06, SEQ-13).</summary>
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/v1/admin/guides")]
public sealed class AdminGuidesController : BaseApiController
{
    /// <summary>Lists guide profiles, optionally filtered by status (for example Pending).</summary>
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] GuideStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(new GetGuideApplicationsQuery(status, page, pageSize), cancellationToken));

    /// <summary>Profile details including verification document metadata.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new GetGuideProfileDetailsQuery(id), cancellationToken));

    /// <summary>Approves a pending profile and grants the TourGuide role.</summary>
    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new ApproveGuideProfileCommand(id), cancellationToken));

    /// <summary>Rejects a pending profile with a reason.</summary>
    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid id,
        [FromBody] RejectGuideProfileRequest request,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new RejectGuideProfileCommand(id, request.Reason), cancellationToken));

    /// <summary>Suspends an approved profile.</summary>
    [HttpPost("{id:guid}/suspend")]
    public async Task<IActionResult> Suspend(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new SuspendGuideProfileCommand(id), cancellationToken));
}
