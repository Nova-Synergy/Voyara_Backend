using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Contracts.Planning;
using Voyara.Application.Trips.Commands.CreateTrip;
using Voyara.Application.Trips.Commands.DeleteTrip;
using Voyara.Application.Trips.Queries.GetTripById;

namespace Voyara.Api.Controllers.Planning;

/// <summary>
/// Tourist trip management endpoints.
/// </summary>
[Route("api/v1/[controller]")]
public sealed class TripsController : BaseApiController
{
    /// <summary>
    /// Gets a trip by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the trip.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new GetTripByIdQuery(id), cancellationToken));

    /// <summary>
    /// Creates a new tourist trip plan.
    /// </summary>
    /// <param name="request">Trip creation details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost]
    public async Task<IActionResult> CreateTrip(
        [FromBody] CreateTripRequest request,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new CreateTripCommand(
                request.Title,
                request.StartDate,
                request.EndDate,
                request.Visibility,
                request.JourneyId),
            cancellationToken));

    /// <summary>
    /// Deletes a trip and revokes associated resources.
    /// </summary>
    /// <param name="id">The unique identifier of the trip to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTrip(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new DeleteTripCommand(id), cancellationToken));
}
