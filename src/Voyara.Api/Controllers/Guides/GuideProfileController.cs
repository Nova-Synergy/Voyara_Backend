
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.DTOs.Guides;
using Voyara.Application.Guides;
using Voyara.Domain.Enums;

namespace Voyara.Api.Controllers.Guides;

/// <summary>
/// Provides API endpoints for managing guide profiles and their related resources,
/// including reviews, requests, languages, expertise, availability, time-off, and pricing.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public sealed class GuidesController : GuidesControllerBase
{
    /// <summary>
    /// Retrieves all guide profiles with pagination.
    /// </summary>
    /// <param name="page">The page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of guide profiles per page. Defaults to 10.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An HTTP 200 response containing the paginated guide profiles.</returns>
    [HttpGet]
    [Route("GetAllGuideProfiles")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        => Map(await Sender.Send(new GetGuideProfilesQuery(page, pageSize), ct), Ok);

    /// <summary>
    /// Retrieves a guide profile by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the guide profile.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An HTTP 200 response containing the requested guide profile.</returns>
    [HttpGet]
    [Route("GetGuideProfileById/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Map(await Sender.Send(new GetGuideProfileQuery(id), ct), Ok);

    /// <summary>
    /// Creates a new guide profile.
    /// </summary>
    /// <param name="dto">The data required to create the guide profile.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An HTTP 201 Created response containing the created guide profile.</returns>
    [HttpPost]
    public async Task<IActionResult> Create(CreateGuideProfileDto dto, CancellationToken ct)
        => Map(await Sender.Send(new CreateGuideProfileCommand(dto), ct),
            value => CreatedAtAction(nameof(GetById), new { id = value.Id }, value));

    /// <summary>
    /// Updates an existing guide profile.
    /// </summary>
    /// <param name="id">The unique identifier of the guide profile to update.</param>
    /// <param name="dto">The updated guide profile data.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An HTTP 204 No Content response when the update succeeds.</returns>
    [HttpPut]
    [Route("UpdateGuideProfile/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateGuideProfileDto dto, CancellationToken ct)
        => Map(await Sender.Send(new UpdateGuideProfileCommand(id, dto), ct));

    /// <summary>
    /// Deletes an existing guide profile.
    /// </summary>
    /// <param name="id">The unique identifier of the guide profile to delete.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An HTTP 204 No Content response when the deletion succeeds.</returns>
    [Authorize]
    [HttpDelete]
    [Route("DeleteGuideProfile/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => Map(await Sender.Send(new DeleteGuideProfileCommand(id), ct));

    /// <summary>
    /// Retrieves the reviews associated with a guide profile using pagination.
    /// </summary>
    /// <param name="id">The unique identifier of the guide profile.</param>
    /// <param name="page">The page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of reviews per page. Defaults to 10.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An HTTP 200 response containing the paginated guide reviews.</returns>
    [HttpGet]
    [Route("{id:guid}/reviews")]
    public async Task<IActionResult> GetReviews(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        => Map(await Sender.Send(new GetGuideReviewsQuery(id, page, pageSize), ct), Ok);

    /// <summary>
    /// Retrieves the requests associated with a guide profile,
    /// with optional status filtering and pagination.
    /// </summary>
    /// <param name="id">The unique identifier of the guide profile.</param>
    /// <param name="status">The optional request status used for filtering.</param>
    /// <param name="page">The page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of requests per page. Defaults to 10.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An HTTP 200 response containing the paginated guide requests.</returns>
    [HttpGet]
    [Route("{id:guid}/requests")]
    public async Task<IActionResult> GetRequests(Guid id, [FromQuery] RequestStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        => Map(await Sender.Send(new GetGuideRequestsQuery(id, status, page, pageSize), ct), Ok);

    /// <summary>
    /// Adds a language to an existing guide profile.
    /// </summary>
    /// <param name="id">The unique identifier of the guide profile.</param>
    /// <param name="dto">The data required to add the guide language.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An HTTP 201 Created response containing the added language.</returns>
    [HttpPost]
    [Route("{id:guid}/languages")]
    public async Task<IActionResult> AddLanguage(Guid id, CreateGuideLanguageDto dto, CancellationToken ct)
        => Map(await Sender.Send(new AddGuideLanguageCommand(id, dto), ct),
            value => CreatedAtAction(nameof(GetById), new { id }, value));

    /// <summary>
    /// Removes a language from an existing guide profile.
    /// </summary>
    /// <param name="id">The unique identifier of the guide profile.</param>
    /// <param name="itemId">The unique identifier of the language to remove.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An HTTP 204 No Content response when the language is removed successfully.</returns>
    [HttpDelete]
    [Route("{id:guid}/languages/{itemId:guid}")]
    public async Task<IActionResult> RemoveLanguage(Guid id, Guid itemId, CancellationToken ct)
        => Map(await Sender.Send(new RemoveGuideLanguageCommand(id, itemId), ct));

    /// <summary>
    /// Adds an area of expertise to an existing guide profile.
    /// </summary>
    /// <param name="id">The unique identifier of the guide profile.</param>
    /// <param name="dto">The data required to add the guide expertise.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An HTTP 201 Created response containing the added expertise.</returns>
    [HttpPost]
    [Route("{id:guid}/expertise")]
    public async Task<IActionResult> AddExpertise(Guid id, CreateGuideExpertiseDto dto, CancellationToken ct)
        => Map(await Sender.Send(new AddGuideExpertiseCommand(id, dto), ct),
            value => CreatedAtAction(nameof(GetById), new { id }, value));

    /// <summary>
    /// Removes an area of expertise from an existing guide profile.
    /// </summary>
    /// <param name="id">The unique identifier of the guide profile.</param>
    /// <param name="itemId">The unique identifier of the expertise item to remove.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An HTTP 204 No Content response when the expertise is removed successfully.</returns>
    [HttpDelete]
    [Route("{id:guid}/expertise/{itemId:guid}")]
    public async Task<IActionResult> RemoveExpertise(Guid id, Guid itemId, CancellationToken ct)
        => Map(await Sender.Send(new RemoveGuideExpertiseCommand(id, itemId), ct));

    /// <summary>
    /// Adds an availability schedule to an existing guide profile.
    /// </summary>
    /// <param name="id">The unique identifier of the guide profile.</param>
    /// <param name="dto">The data required to add the guide availability.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An HTTP 201 Created response containing the added availability.</returns>
    [HttpPost]
    [Route("{id:guid}/availability")]
    public async Task<IActionResult> AddAvailability(Guid id, CreateGuideAvailabilityDto dto, CancellationToken ct)
        => Map(await Sender.Send(new AddGuideAvailabilityCommand(id, dto), ct),
            value => CreatedAtAction(nameof(GetById), new { id }, value));

    /// <summary>
    /// Removes an availability schedule from an existing guide profile.
    /// </summary>
    /// <param name="id">The unique identifier of the guide profile.</param>
    /// <param name="itemId">The unique identifier of the availability item to remove.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An HTTP 204 No Content response when the availability is removed successfully.</returns>
    [HttpDelete]
    [Route("{id:guid}/availability/{itemId:guid}")]
    public async Task<IActionResult> RemoveAvailability(Guid id, Guid itemId, CancellationToken ct)
        => Map(await Sender.Send(new RemoveGuideAvailabilityCommand(id, itemId), ct));

    /// <summary>
    /// Adds a time-off period to an existing guide profile.
    /// </summary>
    /// <param name="id">The unique identifier of the guide profile.</param>
    /// <param name="dto">The data required to add the guide time-off period.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An HTTP 201 Created response containing the added time-off period.</returns>
    [HttpPost]
    [Route("{id:guid}/time-off")]
    public async Task<IActionResult> AddTimeOff(Guid id, CreateGuideTimeOffDto dto, CancellationToken ct)
        => Map(await Sender.Send(new AddGuideTimeOffCommand(id, dto), ct),
            value => CreatedAtAction(nameof(GetById), new { id }, value));

    /// <summary>
    /// Removes a time-off period from an existing guide profile.
    /// </summary>
    /// <param name="id">The unique identifier of the guide profile.</param>
    /// <param name="itemId">The unique identifier of the time-off item to remove.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An HTTP 204 No Content response when the time-off period is removed successfully.</returns>
    [HttpDelete]
    [Route("{id:guid}/time-off/{itemId:guid}")]
    public async Task<IActionResult> RemoveTimeOff(Guid id, Guid itemId, CancellationToken ct)
        => Map(await Sender.Send(new RemoveGuideTimeOffCommand(id, itemId), ct));

    /// <summary>
    /// Adds a pricing option to an existing guide profile.
    /// </summary>
    /// <param name="id">The unique identifier of the guide profile.</param>
    /// <param name="dto">The data required to add the guide pricing option.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An HTTP 201 Created response containing the added pricing option.</returns>
    [HttpPost]
    [Route("{id:guid}/pricing")]
    public async Task<IActionResult> AddPricing(Guid id, CreateGuidePricingDto dto, CancellationToken ct)
        => Map(await Sender.Send(new AddGuidePricingCommand(id, dto), ct),
            value => CreatedAtAction(nameof(GetById), new { id }, value));

    /// <summary>
    /// Removes a pricing option from an existing guide profile.
    /// </summary>
    /// <param name="id">The unique identifier of the guide profile.</param>
    /// <param name="itemId">The unique identifier of the pricing option to remove.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An HTTP 204 No Content response when the pricing option is removed successfully.</returns>
    [HttpDelete]
    [Route("{id:guid}/pricing/{itemId:guid}")]
    public async Task<IActionResult> RemovePricing(Guid id, Guid itemId, CancellationToken ct)
        => Map(await Sender.Send(new RemoveGuidePricingCommand(id, itemId), ct));
}
