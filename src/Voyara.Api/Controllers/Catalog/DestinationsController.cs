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
    /// <summary>Searches published destinations by free text.</summary>
    /// <param name="searchTerm">Text matched against destination names and descriptions.</param>
    /// <param name="lang">Language of the returned texts (for example en or ar); defaults to English.</param>
    /// <param name="page">One-based page number.</param>
    /// <param name="pageSize">Items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] string? lang,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(
            new SearchDestinationsQuery(searchTerm, lang, page, pageSize), cancellationToken));

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
