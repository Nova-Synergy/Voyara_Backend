using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Contracts.Planning;
using Voyara.Application.Planning.Commands.AddJourneyItem;
using Voyara.Application.Planning.Commands.GenerateJourney;
using Voyara.Application.Planning.Commands.MoveJourneyItem;
using Voyara.Application.Planning.Commands.RemoveJourneyItem;
using Voyara.Application.Planning.Queries.GetJourneyById;
using Voyara.Application.Planning.Queries.ValidateJourney;

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
    /// Validates business invariants, daily capacity, and scheduling feasibility for a journey.
    /// </summary>
    /// <param name="id">The unique identifier of the journey to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("{id:guid}/validate")]
    public async Task<IActionResult> Validate(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new ValidateJourneyQuery(id), cancellationToken));

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

    /// <summary>
    /// Moves an itinerary stop item to another day or sequence order within the journey.
    /// </summary>
    /// <param name="id">The unique identifier of the journey.</param>
    /// <param name="itemId">The unique identifier of the item to move.</param>
    /// <param name="request">Target day number and order.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPut("{id:guid}/items/{itemId:guid}/move")]
    public async Task<IActionResult> MoveItem(
        Guid id,
        Guid itemId,
        [FromBody] MoveJourneyItemRequest request,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new MoveJourneyItemCommand(id, itemId, request.TargetDayNumber, request.NewOrder),
            cancellationToken));

    /// <summary>
    /// Adds a destination stop to an itinerary day in the journey.
    /// </summary>
    /// <param name="id">The unique identifier of the journey.</param>
    /// <param name="request">Stop details (destination, day number, time window, transport).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("{id:guid}/items")]
    public async Task<IActionResult> AddItem(
        Guid id,
        [FromBody] AddJourneyItemRequest request,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new AddJourneyItemCommand(
                id,
                request.DayNumber,
                request.DestinationId,
                request.CityId,
                request.StartTime,
                request.EndTime,
                request.Transport),
            cancellationToken));

    /// <summary>
    /// Removes a destination stop from the journey itinerary.
    /// </summary>
    /// <param name="id">The unique identifier of the journey.</param>
    /// <param name="itemId">The unique identifier of the stop to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpDelete("{id:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItem(
        Guid id,
        Guid itemId,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new RemoveJourneyItemCommand(id, itemId), cancellationToken));
}
