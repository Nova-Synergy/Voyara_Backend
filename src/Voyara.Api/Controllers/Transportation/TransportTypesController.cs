using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Application.Transportation.Queries.ListTransportTypes;

namespace Voyara.Api.Controllers.Transportation;

/// <summary>Read endpoint for transport types.</summary>
[Authorize]
[Route("api/v1/[controller]")]
public sealed class TransportTypesController : BaseApiController
{
    /// <summary>Lists all transport types.</summary>
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new ListTransportTypesQuery(), cancellationToken));
}
