using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Voyara.Api.Authorization;
using Voyara.Application.Localization.Commands.SetDestinationTranslation;
using Voyara.Application.Localization.Commands.RemoveDestinationTranslation;
using Voyara.Application.Localization.Queries.GetDestinationTranslations;

namespace Voyara.Api.Controllers.Localization;

[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/v1/admin/destinations/{destinationId:guid}/translations")]
public sealed class DestinationTranslationsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        Guid destinationId,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new GetDestinationTranslationsQuery(destinationId),
            cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("{languageCode}")]
    public async Task<IActionResult> SetTranslation(
        Guid destinationId,
        string languageCode,
        SetDestinationTranslationCommand command,
        CancellationToken cancellationToken)
    {
        if (destinationId != command.DestinationId ||
            !string.Equals(languageCode, command.LanguageCode,
                StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Route values do not match command values.");
        }

        var result = await Sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("{languageCode}")]
    public async Task<IActionResult> RemoveTranslation(
        Guid destinationId,
        string languageCode,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new RemoveDestinationTranslationCommand(
                destinationId, languageCode),
            cancellationToken);

        return HandleResult(result);
    }
}