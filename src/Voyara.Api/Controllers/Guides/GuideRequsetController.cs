
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.DTOs.Guides;
using Voyara.Application.Guides;
using Voyara.Domain.Enums;

namespace Voyara.Api.Controllers.Guides;

/// <summary>
/// Provides API endpoints for managing guide requests,
/// including retrieving, creating, and reviewing requests.
/// </summary>
[ApiController]
[Route("api/v1/guide-requests")]
public sealed class GuideRequestsController : GuidesControllerBase
{
    /// <summary>
    /// Retrieves the guide requests belonging to the currently authenticated user,
    /// with optional status filtering and pagination.
    /// </summary>
    /// <param name="status">The optional status used to filter guide requests.</param>
    /// <param name="page">The page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of requests per page. Defaults to 10.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// An HTTP 200 response containing the paginated guide requests
    /// when the operation succeeds.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetMine(
        [FromQuery] RequestStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
        => Map(await Sender.Send(new GetMyGuideRequestsQuery(status, page, pageSize), ct), Ok);

    /// <summary>
    /// Retrieves a guide request by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the guide request.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// An HTTP 200 response containing the guide request when found.
    /// </returns>
    [HttpGet]
    [Route("GetGuideRequestById/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Map(await Sender.Send(new GetGuideRequestQuery(id), ct), Ok);

    /// <summary>
    /// Creates a new guide request.
    /// </summary>
    /// <param name="dto">The data required to create the guide request.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// An HTTP 201 Created response containing the created guide request
    /// and its resource location when the operation succeeds.
    /// </returns>
    [HttpPost]
    [Route("CreateGuideRequest")]
    public async Task<IActionResult> Create(CreateGuideRequestDto dto, CancellationToken ct)
        => Map(await Sender.Send(new CreateGuideRequestCommand(dto), ct),
            value => CreatedAtAction(nameof(GetById), new { id = value.Id }, value));

    /// <summary>
    /// Creates a review for a guide request.
    /// </summary>
    /// <param name="id">The unique identifier of the guide request to review.</param>
    /// <param name="dto">The data required to create the guide review.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// An HTTP 201 Created response containing the created review
    /// when the operation succeeds.
    /// </returns>
    [HttpPost]
    [Route("{id:guid}/review")]
    public async Task<IActionResult> CreateReview(Guid id, CreateGuideReviewDto dto, CancellationToken ct)
        => Map(await Sender.Send(new CreateGuideReviewCommand(id, dto), ct),
            value => CreatedAtAction(nameof(GetById), new { id }, value));
}
