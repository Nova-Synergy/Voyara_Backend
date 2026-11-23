using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Contracts.Planning;
using Voyara.Application.CostEstimation.Queries.EstimateTransportationCost;
using Voyara.Application.Planning.Commands.AddJourneyItem;
using Voyara.Application.Planning.Commands.CustomizeJourney;
using Voyara.Application.Planning.Commands.GenerateJourney;
using Voyara.Application.Planning.Commands.MoveJourneyItem;
using Voyara.Application.Planning.Commands.RemoveJourneyItem;
using Voyara.Application.Planning.Commands.UpdateJourney;
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

    /// <summary>
    /// Updates journey title or status.
    /// </summary>
    /// <param name="id">The unique identifier of the journey.</param>
    /// <param name="request">Update parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateJourneyRequest request,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new UpdateJourneyCommand(id, request.Title, request.Status), cancellationToken));

    /// <summary>
    /// Customizes an existing journey using natural language instructions.
    /// </summary>
    /// <param name="id">The unique identifier of the journey.</param>
    /// <param name="request">Customization instruction.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("{id:guid}/customize")]
    public async Task<IActionResult> Customize(
        Guid id,
        [FromBody] CustomizeJourneyRequest request,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new CustomizeJourneyCommand(id, request.Instruction), cancellationToken));

    /// <summary>
    /// Calculates transportation cost estimates for a journey itinerary.
    /// </summary>
    /// <param name="id">The unique identifier of the journey.</param>
    /// <param name="currency">Optional currency code (defaults to USD).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("{id:guid}/cost/transportation")]
    public async Task<IActionResult> EstimateTransportationCost(
        Guid id,
        [FromQuery] string? currency,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new EstimateTransportationCostQuery(id, currency), cancellationToken));
}
