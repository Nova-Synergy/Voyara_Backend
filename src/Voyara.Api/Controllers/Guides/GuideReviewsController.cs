using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Contracts.Guides;

namespace Voyara.Api.Controllers.Guides
{

    /// <summary>Reviews written by tourists for completed sessions. The list per guide is <c>GET api/v1/guides/{id}/reviews</c>.</summary>
    [ApiController]
    [Route("api/v1/guide-reviews")]
    public sealed class GuideReviewsController : BaseApiController
    {
        /// <summary>Creates a review for a completed request owned by the current tourist.</summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateGuideReviewRequest request, CancellationToken cancellationToken)
        {
            var result = await Sender.Send(
                new CreateGuideReviewCommand(request.GuideRequestId, request.Rating, request.Text), cancellationToken);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
                : HandleResult(result);
        }

        /// <summary>Gets one review.</summary>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
            => HandleResult(await Sender.Send(new GetGuideReviewByIdQuery(id), cancellationToken));
    }

}
