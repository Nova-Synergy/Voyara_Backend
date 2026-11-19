using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Authorization;
using Voyara.Application.Geography.Commands.ActivateCity;
using Voyara.Application.Geography.Commands.CreateCity;
using Voyara.Application.Geography.Commands.DeactivateCity;
using Voyara.Application.Geography.Commands.UpdateCity;
using Voyara.Application.Geography.Queries.GetCities;
using Voyara.Application.Geography.Queries.GetCityById;

namespace Voyara.Api.Controllers.Geography;

[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/v1/admin/cities")]
public sealed class CitiesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? countryId,
        [FromQuery] bool? isActive,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new GetCitiesQuery(countryId, isActive), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new GetCityByIdQuery(id), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCityCommand command,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(command, cancellationToken));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateCityCommand command,
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
            new ActivateCityCommand(id), cancellationToken));

    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> Deactivate(
        int id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new DeactivateCityCommand(id), cancellationToken));
}