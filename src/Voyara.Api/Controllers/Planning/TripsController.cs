using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Contracts.Planning;
using Voyara.Application.Trips.Commands.CreateTrip;
using Voyara.Application.Trips.Commands.DeleteTrip;
using Voyara.Application.Trips.Commands.UpdateTrip;
using Voyara.Application.Trips.Queries.GetMyTrips;
using Voyara.Application.Trips.Queries.GetTripById;
using Voyara.Domain.Enums;

namespace Voyara.Api.Controllers.Planning;

/// <summary>
/// Tourist trip management endpoints.
/// </summary>
[Route("api/v1/[controller]")]
public sealed class TripsController : BaseApiController
{
    /// <summary>
    /// Retrieves a paginated list of trips belonging to the authenticated tourist.
    /// </summary>
    /// <param name="page">1-based page index (defaults to 1).</param>
    /// <param name="pageSize">Number of items per page (defaults to 10).</param>
    /// <param name="status">Optional status filter (e.g. Draft, Upcoming, Ongoing, Completed, Cancelled).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet]
    public async Task<IActionResult> GetMyTrips(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] TripStatus? status = null,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(new GetMyTripsQuery(page, pageSize, status), cancellationToken));

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
    /// Updates an existing trip's details, schedule, visibility, or status.
    /// </summary>
    /// <param name="id">The unique identifier of the trip to update.</param>
    /// <param name="request">The updated trip information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTrip(
        Guid id,
        [FromBody] UpdateTripRequest request,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new UpdateTripCommand(
                id,
                request.Title,
                request.StartDate,
                request.EndDate,
                request.Visibility,
                request.Status),
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
