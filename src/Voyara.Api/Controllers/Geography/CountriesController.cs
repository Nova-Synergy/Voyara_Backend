using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Authorization;
using Voyara.Application.Geography.Commands.ActivateCountry;
using Voyara.Application.Geography.Commands.CreateCountry;
using Voyara.Application.Geography.Commands.DeactivateCountry;
using Voyara.Application.Geography.Commands.UpdateCountry;
using Voyara.Application.Geography.Queries.GetCountries;
using Voyara.Application.Geography.Queries.GetCountryById;
namespace Voyara.Api.Controllers.Geography;

[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/v1/admin/countries")]
public sealed class CountriesController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCountryCommand command,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(command, cancellationToken));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateCountryCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match command ID.");

        return HandleResult(await Sender.Send(command, cancellationToken));
    }

    [HttpPatch("{id:int}/activate")]
    public async Task<IActionResult> Activate(
        int id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new ActivateCountryCommand(id), cancellationToken));

    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> Deactivate(
        int id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new DeactivateCountryCommand(id), cancellationToken));
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? isActive,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new GetCountriesQuery(isActive, search), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new GetCountryByIdQuery(id), cancellationToken));
}