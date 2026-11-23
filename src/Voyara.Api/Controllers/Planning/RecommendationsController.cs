using Microsoft.AspNetCore.Mvc;
using Voyara.Application.DTOs;
using Voyara.Application.Recommendations.Queries.GetDestinationRecommendations;
using Voyara.Application.Recommendations.Queries.GetJourneyRecommendations;

namespace Voyara.Api.Controllers.Planning;

/// <summary>
/// AI and popularity-based recommendation endpoints.
/// </summary>
[Route("api/v1/[controller]")]
public sealed class RecommendationsController : BaseApiController
{
    /// <summary>
    /// Retrieves personalized or trending destination recommendations.
    /// </summary>
    /// <param name="limit">Maximum recommendations to return (defaults to 5).</param>
    /// <param name="cityId">Optional city filter.</param>
    /// <param name="categoryId">Optional category filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("destinations")]
    public async Task<IActionResult> GetDestinationRecommendations(
        [FromQuery] int limit = 5,
        [FromQuery] Guid? cityId = null,
        [FromQuery] Guid? categoryId = null,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(
            new GetDestinationRecommendationsQuery(limit, cityId, categoryId),
            cancellationToken));

    /// <summary>
    /// Retrieves personalized or trending journey recommendations.
    /// </summary>
    /// <param name="limit">Maximum recommendations to return (defaults to 5).</param>
    /// <param name="totalDays">Optional duration filter in days.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("journeys")]
    public async Task<IActionResult> GetJourneyRecommendations(
        [FromQuery] int limit = 5,
        [FromQuery] int? totalDays = null,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(
            new GetJourneyRecommendationsQuery(limit, totalDays),
            cancellationToken));
}
