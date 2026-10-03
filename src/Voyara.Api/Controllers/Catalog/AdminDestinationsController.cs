using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Authorization;
using Voyara.Application.Catalog.Commands.ArchiveDestination;
using Voyara.Application.Catalog.Commands.PublishDestination;

namespace Voyara.Api.Controllers.Catalog;

/// <summary>Admin lifecycle endpoints for destinations (Catalog reference slice).</summary>
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/v1/admin/[controller]")]
public sealed class AdminDestinationsController : BaseApiController
{
    /// <summary>Publishes a draft destination.</summary>
    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new PublishDestinationCommand(id), cancellationToken));

    /// <summary>Archives a destination.</summary>
    [HttpPost("{id:guid}/archive")]
    public async Task<IActionResult> Archive(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new ArchiveDestinationCommand(id), cancellationToken));
}
