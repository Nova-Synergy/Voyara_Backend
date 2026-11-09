using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Contracts.Planning;
using Voyara.Application.Planning.Commands.GenerateJourney;
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

    /// <summary>
    /// Generates a new AI-synthesized journey itinerary from travel requirements.
    /// </summary>
    /// <param name="request">The journey generation parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("generate")]
    public async Task<IActionResult> Generate(
        [FromBody] GenerateJourneyRequest request,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new GenerateJourneyCommand(
                request.Title,
                request.DurationDays,
                request.Interests ?? Array.Empty<string>(),
                request.TravelStyle ?? "Balanced",
                request.TransportPreference ?? "Walking",
                request.Country,
                request.BudgetAmount,
                request.BudgetCurrency),
            cancellationToken));
}
