using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Voyara.Api.Authorization;
using Voyara.Api.Controllers;
using Voyara.Application.Localization.Commands.CreateLanguage;
using Voyara.Application.Localization.Commands.UpdateLanguage;
using Voyara.Application.Localization.Commands.DeleteLanguage;
using Voyara.Application.Localization.Queries.GetAllLanguages;
using Voyara.Application.Localization.Queries.GetLanguageByCode;

namespace Voyara.Api.Controllers.Localization;

[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/v1/admin/languages")]
public sealed class LanguagesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new GetAllLanguagesQuery(), cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> GetByCode(
        string code,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new GetLanguageByCodeQuery(code), cancellationToken);

        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateLanguageCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("{code}")]
    public async Task<IActionResult> Update(
        string code,
        UpdateLanguageCommand command,
        CancellationToken cancellationToken)
    {
        if (!string.Equals(
            code, command.Code,
            StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Route code does not match command code.");
        }

        var result = await Sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("{code}")]
    public async Task<IActionResult> Delete(
        string code,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new DeleteLanguageCommand(code), cancellationToken);

        return HandleResult(result);
    }
}