using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Authorization;
using Voyara.Api.Contracts.Culture_Rules;
using Voyara.Application.Culture_Rules.Commands.CreateLocalRule;
using Voyara.Application.Culture_Rules.Commands.DeleteLocalRule;
using Voyara.Application.Culture_Rules.Commands.UpdateLocalRule;

namespace Voyara.Api.Controllers.Culture_Rules;

/// <summary>Admin endpoints that manage local rules.</summary>
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/v1/admin/[controller]")]
public sealed class AdminLocalRulesController : BaseApiController
{
    /// <summary>Creates a local rule with its first translation.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateLocalRuleRequest request,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new CreateLocalRuleCommand(
                request.Type,
                request.ScopeType,
                request.ScopeTargetId,
                request.IsOfficial,
                request.Source,
                request.LanguageCode,
                request.Title,
                request.Content),
            cancellationToken));

    /// <summary>Updates the category, the official flag and the source of a local rule.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateLocalRuleRequest request,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new UpdateLocalRuleCommand(id, request.Type, request.IsOfficial, request.Source),
            cancellationToken));

    /// <summary>Deletes a local rule.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new DeleteLocalRuleCommand(id), cancellationToken));
}
