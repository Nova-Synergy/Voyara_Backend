using Microsoft.AspNetCore.Mvc;
using Voyara.Application.Trips.Commands.DeleteTrip;

namespace Voyara.Api.Controllers.Planning;

/// <summary>
/// Tourist trip management endpoints.
/// </summary>
[Route("api/v1/[controller]")]
public sealed class TripsController : BaseApiController
{
    /// <summary>
    /// Deletes a trip and revokes associated resources.
    /// </summary>
    /// <param name="id">The unique identifier of the trip to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTrip(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new DeleteTripCommand(id), cancellationToken));
}
