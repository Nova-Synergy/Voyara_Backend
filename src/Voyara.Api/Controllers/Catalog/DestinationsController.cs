using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Contracts.Catalog;
using Voyara.Application.Catalog.Commands.CreateDestination;
using Voyara.Application.Catalog.Queries.GetDestinationById;
using Voyara.Application.Catalog.Queries.SearchDestinations;

namespace Voyara.Api.Controllers.Catalog;

/// <summary>Public destination endpoints (Catalog reference slice).</summary>
[Route("api/v1/[controller]")]
public sealed class DestinationsController : BaseApiController
{
    /// <summary>Searches destinations with paging.</summary>
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(new SearchDestinationsQuery(searchTerm, page, pageSize), cancellationToken));

    /// <summary>Gets a single destination by identifier.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new GetDestinationByIdQuery(id), cancellationToken));

    /// <summary>Creates a new destination in Draft status.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(CreateDestinationRequest request, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new CreateDestinationCommand(request.Name, request.Description, request.Latitude, request.Longitude),
            cancellationToken));
}
