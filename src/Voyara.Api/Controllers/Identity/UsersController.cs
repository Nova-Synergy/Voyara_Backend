using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Authorization;
using Voyara.Application.Identity.Queries.GetCurrentUser;

namespace Voyara.Api.Controllers.Identity;

/// <summary>Endpoints about the authenticated account.</summary>
[Authorize(Policy = AuthorizationPolicies.Authenticated)]
[Route("api/v1/users")]
public sealed class UsersController : BaseApiController
{
    /// <summary>Returns the profile of the caller identified by the bearer token.</summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new GetCurrentUserQuery(), cancellationToken));
}
