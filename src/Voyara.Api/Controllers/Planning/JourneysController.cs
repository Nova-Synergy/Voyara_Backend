using Microsoft.AspNetCore.Mvc;
using Voyara.Application.Planning.Queries.GetJourneyById;

namespace Voyara.Api.Controllers.Planning;

/// <summary>
/// Journey itinerary management endpoints.
/// </summary>
[Route("api/v1/[controller]")]
public sealed class JourneysController : BaseApiController
{
    /// <summary>
    /// Gets a journey itinerary by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the journey.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new GetJourneyByIdQuery(id), cancellationToken));
}
