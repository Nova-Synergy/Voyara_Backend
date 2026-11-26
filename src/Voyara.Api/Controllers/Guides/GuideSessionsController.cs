using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Voyara.Api.Controllers.Guides
{
    /// <summary>Read access to guide sessions created by accepted requests.</summary>
    [ApiController]
    [Authorize]
    [Route("api/v1/guide-sessions")]
    public sealed class GuideSessionsController : BaseApiController
    {
        /// <summary>Gets a session (its two participants only).</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
            => HandleResult(await Sender.Send(new GetGuideSessionByIdQuery(id), cancellationToken));
    }
}
