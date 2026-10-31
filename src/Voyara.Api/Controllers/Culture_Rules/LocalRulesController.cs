using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Application.Culture_Rules.Queries.GetLocalRuleById;
using Voyara.Application.Culture_Rules.Queries.ListLocalRulesByScope;
using Voyara.Domain.Enums;

namespace Voyara.Api.Controllers.Culture_Rules;

/// <summary>Read endpoints for local rules.</summary>
[Authorize]
[Route("api/v1/[controller]")]
public sealed class LocalRulesController : BaseApiController
{
    /// <summary>Gets a local rule with its translations.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new GetLocalRuleByIdQuery(id), cancellationToken));

    /// <summary>Lists the rules of one scope (country, city or destination).</summary>
    [HttpGet]
    public async Task<IActionResult> ListByScope(
        [FromQuery] ScopeType scopeType,
        [FromQuery] string targetId,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new ListLocalRulesByScopeQuery(scopeType, targetId), cancellationToken));
}
